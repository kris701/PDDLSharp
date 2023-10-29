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
    public class GreedyBFS : BaseSearch
    {
        public GreedyBFS(PDDLDecl decl) : base(decl)
        {
        }

        internal override ActionPlan Solve(IHeuristic h, IState state)
        {
            Expanded = 0;
            Generated = 0;

            var closedList = new HashSet<StateMove>();
            var openList = InitializeQueue(h, state);
            while (!_abort && openList.Count > 0)
            {
                var stateMove = openList.Dequeue();
                if (stateMove.State.IsInGoal())
                    return new ActionPlan(stateMove.Steps);
                closedList.Add(stateMove);

                foreach (var act in GroundedActions)
                {
                    if (_abort) break;
                    if (stateMove.State.IsNodeTrue(act.Preconditions))
                    {
                        var check = GenerateNewState(stateMove.State, act);
                        var newMove = new StateMove(check);
                        if (newMove.State.IsInGoal())
                            return new ActionPlan(new List<GroundedAction>(stateMove.Steps) { new GroundedAction(act, act.Parameters.Values) });
                        if (!closedList.Contains(newMove) && !openList.Contains(newMove))
                        {
                            var value = h.GetValue(stateMove.hValue, check, GroundedActions);
                            newMove.Steps = new List<GroundedAction>(stateMove.Steps) { new GroundedAction(act, act.Parameters.Values) };
                            newMove.hValue = value;
                            openList.Enqueue(newMove, value);
                        }
                    }
                }

                Expanded++;
            }
            throw new Exception("No solution found!");
        }
    }
}
