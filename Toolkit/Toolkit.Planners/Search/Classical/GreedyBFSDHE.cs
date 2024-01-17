using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;
using PDDLSharp.StateSpaces.SAS;

namespace PDDLSharp.Toolkit.Planners.Search.Classical
{
    /// <summary>
    /// Greedy Best First Search with Deferred Heuristic Evaluation
    /// (<seealso href="https://ai.dmi.unibas.ch/papers/helmert-jair06.pdf">Helmert 2006</seealso>)
    /// </summary>
    public class GreedyBFSDHE : BaseSearch
    {
        public GreedyBFSDHE(SASDecl decl, IHeuristic heuristic) : base(decl, heuristic)
        {
        }

        internal override ActionPlan? Solve(IHeuristic h, ISASState state)
        {
            while (!Aborted && _openList.Count > 0)
            {
                var stateMove = ExpandBestState();
                if (!stateMove.Evaluated)
                    stateMove.hValue = h.GetValue(stateMove, stateMove.State, Declaration.Operators);

                bool lowerFound = false;
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
                            if (lowerFound)
                            {
                                newMove.Steps = new List<GroundedAction>(stateMove.Steps) { GenerateFromOp(op) };
                                newMove.hValue = stateMove.hValue;
                                newMove.Evaluated = false;
                                _openList.Enqueue(newMove, stateMove.hValue);
                            }
                            else
                            {
                                var value = h.GetValue(stateMove, newMove.State, Declaration.Operators);
                                if (value < stateMove.hValue)
                                    lowerFound = true;
                                newMove.Steps = new List<GroundedAction>(stateMove.Steps) { GenerateFromOp(op) };
                                newMove.hValue = value;
                                _openList.Enqueue(newMove, value);
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}
