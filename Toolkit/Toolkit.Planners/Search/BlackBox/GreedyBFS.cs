using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;
using PDDLSharp.StateSpaces.SAS;
using PDDLSharp.Toolkit.Planners.Tools;

namespace PDDLSharp.Toolkit.Planners.Search.BlackBox
{
    public class GreedyBFS : BaseBlackBoxSearch
    {
        public GreedyBFS(SASDecl decl, IHeuristic heuristic) : base(decl, heuristic)
        {
        }

        internal override ActionPlan? Solve(IHeuristic h, ISASState state)
        {
            while (!Aborted && _openList.Count > 0)
            {
                var stateMove = ExpandBestState();
                var applicables = GetApplicables(stateMove.State);
                foreach (var op in applicables)
                {
                    if (Aborted) break;
                    var newMove = new StateMove(Simulate(stateMove.State, op));
                    if (newMove.State.IsInGoal())
                        return new ActionPlan(new List<GroundedAction>(stateMove.Steps) { GenerateFromOp(op) });
                    if (!_closedList.Contains(newMove) && !_openList.Contains(newMove))
                    {
                        var value = h.GetValue(stateMove, newMove.State, new List<Operator>());
                        newMove.Steps = new List<GroundedAction>(stateMove.Steps) { GenerateFromOp(op) };
                        newMove.hValue = value;
                        _openList.Enqueue(newMove, value);
                    }
                }
            }
            return null;
        }
    }
}
