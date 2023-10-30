using PDDLSharp.Models;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Planners.Exceptions;
using PDDLSharp.Toolkit.StateSpace;

namespace PDDLSharp.Toolkit.Planners.Search
{
    public class GreedyBFS : BaseSearch
    {
        public GreedyBFS(PDDLDecl decl) : base(decl)
        {
        }

        internal override ActionPlan Solve(IHeuristic h, IState<Fact, Operator> state)
        {
            while (!_abort && _openList.Count > 0)
            {
                var stateMove = ExpandBestState();
                if (stateMove.State.IsInGoal())
                    return new ActionPlan(stateMove.Steps);

                foreach (var op in Operators)
                {
                    if (_abort) break;
                    if (stateMove.State.IsNodeTrue(op))
                    {
                        var newMove = new StateMove(GenerateNewState(stateMove.State, op));
                        if (newMove.State.IsInGoal())
                            return new ActionPlan(new List<GroundedAction>(stateMove.Steps) { GenerateFromOp(op) });
                        if (!_closedList.Contains(newMove) && !_openList.Contains(newMove))
                        {
                            var value = h.GetValue(stateMove, newMove.State, Operators);
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
