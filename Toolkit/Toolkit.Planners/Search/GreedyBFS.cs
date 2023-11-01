using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Planners.Exceptions;
using PDDLSharp.Toolkit.StateSpace.SAS;

namespace PDDLSharp.Toolkit.Planners.Search
{
    public class GreedyBFS : BaseSearch
    {
        public GreedyBFS(SASDecl decl, IHeuristic heuristic) : base(decl, heuristic)
        {
        }

        internal override ActionPlan Solve(IHeuristic h, ISASState state)
        {
            while (!Aborted && _openList.Count > 0)
            {
                var stateMove = ExpandBestState();
                foreach (var op in Declaration.Operators)
                {
                    if (Aborted) break;
                    if (stateMove.State.IsNodeTrue(op))
                    {
                        var newMove = new StateMove(GenerateNewState(stateMove.State, op));
                        if (newMove.State.IsInGoal())
                            return new ActionPlan(new List<GroundedAction>(stateMove.Steps) { GenerateFromOp(op) });
                        if (!_closedList.Contains(newMove) && !_openList.Contains(newMove))
                        {
                            var value = h.GetValue(stateMove, newMove.State, Declaration.Operators);
                            newMove.Steps = new List<GroundedAction>(stateMove.Steps) { GenerateFromOp(op) };
                            newMove.hValue = value;
                            _openList.Enqueue(newMove, value);
                        }
                    }
                }
            }
            throw new NoSolutionFoundException();
        }
    }
}
