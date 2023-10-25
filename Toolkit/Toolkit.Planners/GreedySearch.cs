using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.Plans;
using PDDLSharp.Toolkit.Grounders;
using PDDLSharp.Toolkit.StateSpace;

namespace PDDLSharp.Toolkit.Planners
{
    public class GreedySearch : IPlanner<PDDLStateSpace>
    {
        public DomainDecl Domain { get; }
        public ProblemDecl Problem { get; }
        public int Generated { get; internal set; }
        public int Expanded { get; internal set; }

        private bool _preprocessed = false;
        private List<ActionDecl> _groundedActions = new List<ActionDecl>();

        public GreedySearch(DomainDecl domain, ProblemDecl problem)
        {
            Domain = domain;
            Problem = problem;
        }

        public void PreProcess()
        {
            if (_preprocessed)
                return;
            IGrounder<ActionDecl> grounder = new ActionGrounder(new PDDLDecl(Domain, Problem));
            _groundedActions = new List<ActionDecl>();
            foreach (var action in Domain.Actions)
                _groundedActions.AddRange(grounder.Ground(action));
            _preprocessed = true;
        }

        public ActionPlan Solve(IHeuristic<PDDLStateSpace> h)
        {
            Expanded = 0;
            Generated = 0;
            if (!_preprocessed)
                PreProcess();

            IState state = new PDDLStateSpace(new PDDLDecl(Domain, Problem));
            HashSet<StateMove> closedList = new HashSet<StateMove>();
            Queue<StateMove> openList = new Queue<StateMove>();
            openList.Enqueue(new StateMove(state, h.GetValue(state)));

            while (openList.Count > 0)
            {
                var stateMove = openList.Dequeue();

                for (int i = 0; i < _groundedActions.Count; i++)
                {
                    if (stateMove.State.IsNodeTrue(_groundedActions[i].Preconditions))
                    {
                        Expanded++;
                        var check = stateMove.State.Copy();
                        check.ExecuteNode(_groundedActions[i].Effects);
                        var value = h.GetValue(check);
                        var newMove = new StateMove(check, new List<GroundedAction>(stateMove.Steps) { new GroundedAction(_groundedActions[i], _groundedActions[i].Parameters.Values) }, value);
                        if (!closedList.Contains(newMove))
                        {
                            if (check.IsInGoal())
                                return new ActionPlan(newMove.Steps, newMove.hValue);
                            else if (value <= stateMove.hValue)
                            {
                                Generated++;
                                openList.Enqueue(newMove);
                            }
                        }
                        closedList.Add(newMove);
                    }
                }
            }
            throw new Exception("No solution found!");
        }
    }
}
