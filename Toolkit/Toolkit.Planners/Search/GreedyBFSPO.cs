﻿using PDDLSharp.Models;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Planners.Exceptions;
using PDDLSharp.Toolkit.Planners.Tools;
using PDDLSharp.Toolkit.StateSpace;
using PDDLSharp.Toolkit.StateSpace.SAS;

namespace PDDLSharp.Toolkit.Planners.Search
{
    /// <summary>
    /// Greedy Best First Search with Preferred Operators
    /// (<seealso href="https://ai.dmi.unibas.ch/papers/helmert-jair06.pdf">Helmert 2006</seealso>).
    /// The preferred operators are extracted from a relaxed plan of the problem
    /// </summary>
    public class GreedyBFSPO : BaseSearch
    {
        private RelaxedPlanGenerator _graphGenerator;
        public GreedyBFSPO(PDDLDecl decl) : base(decl)
        {
            _graphGenerator = new RelaxedPlanGenerator(decl);
        }

        internal override ActionPlan Solve(IHeuristic h, IState<Fact, Operator> state)
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

                    foreach (var op in preferedOperators)
                    {
                        if (_abort) break;
                        if (stateMove.State.IsNodeTrue(op))
                        {
                            var newMove = new StateMove(GenerateNewState(stateMove.State, op));
                            if (newMove.State.IsInGoal())
                                return new ActionPlan(new List<GroundedAction>(stateMove.Steps) { GenerateFromOp(op) });
                            if (!_closedList.Contains(newMove) && !preferredQueue.Contains(newMove))
                            {
                                var value = h.GetValue(stateMove, newMove.State, Operators);
                                newMove.Steps = new List<GroundedAction>(stateMove.Steps) { GenerateFromOp(op) };
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
                                preferredQueue.Enqueue(newMove, value);
                                _openList.Enqueue(newMove, value);
                            }
                        }
                    }
                }
            }
            throw new NoSolutionFoundException();
        }

        private HashSet<Operator> GetPreferredOperators()
        {
            var operators = _graphGenerator.GenerateReplaxedPlan(
                new SASStateSpace(Declaration),
                Operators
                );
            if (_graphGenerator.Failed)
                throw new Exception("No relaxed plan could be found from the initial state! Could indicate the problem is unsolvable.");
            return operators;
        }
    }
}