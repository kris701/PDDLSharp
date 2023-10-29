using PDDLSharp.Models.Plans;
using PDDLSharp.Models;
using PDDLSharp.Toolkit.Planners.Exceptions;
using PDDLSharp.Toolkit.StateSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Toolkit.Planners.Tools;

namespace PDDLSharp.Toolkit.Planners.Search
{
    /// <summary>
    /// Greedy Best First Search with Preferred Operators (Helmert 2006)
    /// The preferred operators are extracted from a relaxed plan of the problem
    /// </summary>
    public class GreedyBFSPO : BaseSearch
    {
        private RelaxedPlanGenerator _graphGenerator;
        public GreedyBFSPO(PDDLDecl decl) : base(decl)
        {
            _graphGenerator = new RelaxedPlanGenerator(decl);
        }

        internal override ActionPlan Solve(IHeuristic h, IState state)
        {
            var preferedOperators = GetPreferredOperators();
            var preferredQueue = InitializeQueue(h, state);

            int iteration = 0;
            while (!_abort && _openList.Count > 0 || preferredQueue.Count > 0)
            {
                if (iteration++ % 2 == 0 && preferredQueue.Count > 0)
                {
                    var stateMove = ExpandBestState(preferredQueue);
                    if (stateMove.State.IsInGoal())
                        return new ActionPlan(stateMove.Steps);

                    foreach (var act in preferedOperators)
                    {
                        if (_abort) break;
                        if (stateMove.State.IsNodeTrue(act.Preconditions))
                        {
                            var newMove = new StateMove(GenerateNewState(stateMove.State, act));
                            if (newMove.State.IsInGoal())
                                return new ActionPlan(new List<GroundedAction>(stateMove.Steps) { new GroundedAction(act, act.Parameters.Values) });
                            if (!_closedList.Contains(newMove) && !preferredQueue.Contains(newMove))
                            {
                                var value = h.GetValue(stateMove, newMove.State, GroundedActions);
                                newMove.Steps = new List<GroundedAction>(stateMove.Steps) { new GroundedAction(act, act.Parameters.Values) };
                                newMove.hValue = value;
                                preferredQueue.Enqueue(newMove, value);
                            }
                        }
                    }
                }
                else
                {
                    var stateMove = ExpandBestState();
                    if (stateMove.State.IsInGoal())
                        return new ActionPlan(stateMove.Steps);

                    foreach (var act in GroundedActions)
                    {
                        if (_abort) break;
                        if (stateMove.State.IsNodeTrue(act.Preconditions))
                        {
                            var newMove = new StateMove(GenerateNewState(stateMove.State, act));
                            if (newMove.State.IsInGoal())
                                return new ActionPlan(new List<GroundedAction>(stateMove.Steps) { new GroundedAction(act, act.Parameters.Values) });
                            if (!_closedList.Contains(newMove) && !_openList.Contains(newMove))
                            {
                                var value = h.GetValue(stateMove, newMove.State, GroundedActions);
                                newMove.Steps = new List<GroundedAction>(stateMove.Steps) { new GroundedAction(act, act.Parameters.Values) };
                                newMove.hValue = value;
                                preferredQueue.Enqueue(newMove, value);
                                _openList.Enqueue(newMove, value);
                            }
                        }
                    }
                }
            }
            throw new NoSolutionFoundException();
        }

        private HashSet<ActionDecl> GetPreferredOperators()
        {
            var operators = _graphGenerator.GenerateReplaxedPlan(
                new RelaxedPDDLStateSpace(Declaration),
                GroundedActions
                );
            if (_graphGenerator.Failed)
                throw new Exception("No relaxed plan could be found from the initial state! Could indicate the problem is unsolvable.");
            return operators;
        }
    }
}
