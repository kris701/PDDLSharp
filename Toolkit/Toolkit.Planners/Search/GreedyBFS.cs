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
            while (!_abort && _openList.Count > 0)
            {
                var stateMove = ExpandBestState();
                if (stateMove.State.IsInGoal())
                    return new ActionPlan(stateMove.Steps);

                foreach (var act in GroundedActions)
                {
                    if (_abort) break;
                    if (stateMove.State.IsNodeTrue(act.Preconditions))
                    {
                        var check = GenerateNewState(stateMove.State, act);
                        var newMove = new StateMove(check);
                        if (newMove.State.IsInGoal())
                            return new ActionPlan(new List<GroundedAction>(stateMove.Steps) { new GroundedAction(act, act.Parameters.Values) });
                        if (!_closedList.Contains(newMove) && !_openList.Contains(newMove))
                        {
                            var value = h.GetValue(stateMove.hValue, check, GroundedActions);
                            newMove.Steps = new List<GroundedAction>(stateMove.Steps) { new GroundedAction(act, act.Parameters.Values) };
                            newMove.hValue = value;
                            _openList.Enqueue(newMove, value);
                        }
                    }
                }
            }
            throw new Exception("No solution found!");
        }
    }
}
