using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.Plans;
using PDDLSharp.Toolkit.Grounders;
using PDDLSharp.Toolkit.StateSpace;
using PDDLSharp.Tools;

namespace PDDLSharp.Toolkit.Planners.Search
{
    public class GreedyBFS : IPlanner
    {
        public DomainDecl Domain { get; }
        public ProblemDecl Problem { get; }
        public HashSet<ActionDecl> GroundedActions { get; set; }
        public int Generated { get; internal set; }
        public int Expanded { get; internal set; }

        private bool _preprocessed = false;

        public GreedyBFS(DomainDecl domain, ProblemDecl problem)
        {
            Domain = domain;
            Problem = problem;
            GroundedActions = new HashSet<ActionDecl>();
        }

        public void PreProcess()
        {
            if (_preprocessed)
                return;
            IGrounder<ActionDecl> grounder = new ActionGrounder(new PDDLDecl(Domain, Problem));
            GroundedActions = new HashSet<ActionDecl>();
            foreach (var action in Domain.Actions)
                GroundedActions.AddRange(grounder.Ground(action).ToHashSet());
            _preprocessed = true;
        }

        public ActionPlan Solve(IHeuristic h)
        {
            IState state = new PDDLStateSpace(new PDDLDecl(Domain, Problem));
            return Solve(h, state);
        }

        public ActionPlan Solve(IHeuristic h, IState state)
        {
            Expanded = 0;
            Generated = 0;

            var closedList = new HashSet<StateMove>();
            var openListRef = new HashSet<StateMove>();
            var openList = new PriorityQueue<StateMove, int>();
            var hValue = h.GetValue(int.MaxValue, state, GroundedActions);
            openList.Enqueue(new StateMove(state, hValue), hValue);

            while (openList.Count > 0)
            {
                var stateMove = openList.Dequeue();
                if (stateMove.State.IsInGoal())
                    return new ActionPlan(stateMove.Steps, stateMove.Steps.Count);
                openListRef.Remove(stateMove);
                closedList.Add(stateMove);

                foreach (var act in GroundedActions)
                {
                    if (stateMove.State.IsNodeTrue(act.Preconditions))
                    {
                        Generated++;
                        var check = stateMove.State.Copy();
                        check.ExecuteNode(act.Effects);
                        var value = h.GetValue(stateMove.hValue, check, GroundedActions);
                        var newMove = new StateMove(check, new List<GroundedAction>(stateMove.Steps) { new GroundedAction(act, act.Parameters.Values) }, value);
                        if (newMove.State.IsInGoal())
                            return new ActionPlan(newMove.Steps, newMove.Steps.Count);
                        if (!closedList.Contains(newMove) && !openListRef.Contains(newMove))
                        {
                            if (value < stateMove.hValue)
                            {
                                openList.Enqueue(newMove, value);
                                openListRef.Add(newMove);
                            }
                            else
                                closedList.Add(newMove);
                        }
                    }
                }

                Expanded++;
            }
            throw new Exception("No solution found!");
        }
    }
}
