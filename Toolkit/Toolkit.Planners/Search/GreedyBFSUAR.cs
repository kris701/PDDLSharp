﻿using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Planners.Exceptions;
using PDDLSharp.Toolkit.Planners.Tools;
using PDDLSharp.Toolkit.StateSpace.SAS;
using PDDLSharp.Tools;
using System.Diagnostics;

namespace PDDLSharp.Toolkit.Planners.Search
{
    // Greedy Search with Under-Approximation Refinement
    public class GreedyBFSUAR : BaseSearch
    {
        public int OperatorsUsed { get; set; }

        private RelaxedPlanGenerator _graphGenerator;
        public GreedyBFSUAR(SASDecl decl, IHeuristic heuristic) : base(decl, heuristic)
        {
            _graphGenerator = new RelaxedPlanGenerator(decl);
        }

        internal override ActionPlan Solve(IHeuristic h, ISASState state)
        {
            // Initial Operator Subset
            var operators = GetInitialOperators();
            _openList = InitializeQueue(Heuristic, state, operators.ToList());

            int itt = 0;
            int secs = 0;
            var watch = new Stopwatch();
            watch.Start();
            while (!Aborted)
            {
                itt++;
                if (watch.ElapsedMilliseconds > 1000)
                {
                    Console.WriteLine($"[t={secs++}] Expansions pr second: {itt}. Evaluated {Evaluations}. Expanded {Expanded}. Generated {Generated}. Ops {operators.Count}");
                    watch.Restart();
                    itt = 0;
                }
                // Refinement Guards
                if (_openList.Count == 0)
                    operators = RefineOperators(operators);

                var stateMove = ExpandBestState();
                int best = stateMove.hValue;
                int current = int.MaxValue;
                foreach (var op in operators)
                {
                    if (Aborted) break;
                    if (stateMove.State.IsNodeTrue(op))
                    {
                        var newMove = new StateMove(GenerateNewState(stateMove.State, op));
                        if (newMove.State.IsInGoal())
                        {
                            OperatorsUsed = operators.Count;
                            return new ActionPlan(new List<GroundedAction>(stateMove.Steps) { GenerateFromOp(op) });
                        }
                        if (!_closedList.Contains(newMove) && !_openList.Contains(newMove))
                        {
                            var value = h.GetValue(stateMove, newMove.State, operators.ToList());
                            if (value < current)
                                current = value;
                            newMove.Steps = new List<GroundedAction>(stateMove.Steps) { GenerateFromOp(op) };
                            newMove.hValue = value;
                            _openList.Enqueue(newMove, value);
                        }
                    }
                }

                //if (current > best || best == int.MaxValue)
                //    operators = RefineOperators(operators);
            }
            throw new NoSolutionFoundException();
        }

        private HashSet<Operator> GetInitialOperators()
        {
            var operators = _graphGenerator.GenerateReplaxedPlan(
                new RelaxedSASStateSpace(Declaration),
                Declaration.Operators
                );
            if (_graphGenerator.Failed)
                throw new Exception("No relaxed plan could be found from the initial state! Could indicate the problem is unsolvable.");
            return operators;
        }

        /// <summary>
        /// From the general idea of the paper:
        /// <list type="bullet">
        ///     <item><description>Select the visited states that have the lowest heuristic value. Generate a new subset of operators, based on relaxed plans starting in those states.​</description></item>
        ///     <item><description>If no new operators was found, repeat the previous but with the states with the next lowest heuristic value.​​</description></item>
        ///     <item><description>If there is still no new operators, do it all again, but simply use all applicable actions from the given states, instead of those from relaxed plans.</description></item>
        /// </list>
        /// </summary>
        /// <param name="operators">Set of unrefined operators</param>
        /// <returns>A set of refined operators</returns>
        /// <exception cref="NoSolutionFoundException">If no operators could be found with either the relaxed plans or just applicable operators, assume the problem is unsolvable.</exception>
        private HashSet<Operator> RefineOperators(HashSet<Operator> operators)
        {
            if (_closedList.Count == 0)
                return operators;

            bool refinedOperatorsFound = false;
            bool lookForApplicaple = false;
            int smallestHValue = -1;
            // Refinement Step 2
            while (!refinedOperatorsFound)
            {
                var smallestItem = _closedList.Where(x => x.hValue > smallestHValue).MinBy(x => x.hValue);
                if (smallestItem == null)
                {
                    // Refinement step 3
                    if (lookForApplicaple && _openList.Count != 0)
                        return operators;

                    if (lookForApplicaple)
                        throw new NoSolutionFoundException();

                    // Refinement Step 4
                    smallestHValue = -1;
                    lookForApplicaple = true;
                }
                else
                {
                    // Refinement Step 1
                    smallestHValue = smallestItem.hValue;
                    var newOperators = new HashSet<Operator>();
                    if (lookForApplicaple)
                        newOperators = GetNewApplicableOperators(smallestHValue, operators);
                    else
                        newOperators = GetNewRelaxedOperators(smallestHValue, operators);

                    if (newOperators.Count > 0)
                    {
                        ReopenClosedStates(newOperators);
                        operators.AddRange(newOperators);
                        refinedOperatorsFound = true;
                    }
                }
            }
            return operators;
        }

        private void ReopenClosedStates(HashSet<Operator> newOperators)
        {
            foreach (var closed in _closedList)
            {
                foreach (var newOperator in newOperators)
                {
                    if (closed.State.IsNodeTrue(newOperator))
                    {
                        _closedList.Remove(closed);
                        _openList.Enqueue(closed, closed.hValue);
                        break;
                    }
                }
            }
        }

        private Dictionary<int, HashSet<Operator>> _relaxedCache = new Dictionary<int, HashSet<Operator>>();
        private HashSet<Operator> GetNewRelaxedOperators(int smallestHValue, HashSet<Operator> operators)
        {
            var allSmallest = _closedList.Where(x => x.hValue == smallestHValue).ToList();
            var relaxedPlanOperators = new HashSet<Operator>();
            foreach (var item in allSmallest)
            {
                if (Aborted) throw new NoSolutionFoundException();
                var hash = item.GetHashCode();
                if (_relaxedCache.ContainsKey(hash))
                    relaxedPlanOperators.AddRange(_relaxedCache[hash].Except(operators).ToHashSet());
                else
                {
                    var newOps = _graphGenerator.GenerateReplaxedPlan(
                        item.State,
                        Declaration.Operators
                        );
                    if (!_graphGenerator.Failed)
                    {
                        _relaxedCache.Add(hash, newOps);
                        relaxedPlanOperators.AddRange(newOps.Except(operators).ToHashSet());
                    }
                }
            }
            return relaxedPlanOperators;
        }

        private Dictionary<int, HashSet<Operator>> _applicableCache = new Dictionary<int, HashSet<Operator>>();
        private HashSet<Operator> GetNewApplicableOperators(int smallestHValue, HashSet<Operator> operators)
        {
            var allSmallest = _closedList.Where(x => x.hValue == smallestHValue).ToList();
            var applicableOperators = new HashSet<Operator>();
            foreach (var item in allSmallest)
            {
                if (Aborted) throw new NoSolutionFoundException();
                var hash = item.GetHashCode();
                if (_applicableCache.ContainsKey(hash))
                    applicableOperators.AddRange(_applicableCache[hash].Except(operators).ToHashSet());
                else
                {
                    _applicableCache.Add(hash, new HashSet<Operator>());
                    foreach (var op in Declaration.Operators)
                    {
                        if (!operators.Contains(op))
                        {
                            if (item.State.IsNodeTrue(op))
                            {
                                applicableOperators.Add(op);
                                _applicableCache[hash].Add(op);
                            }
                        }
                    }
                }
            }
            return applicableOperators;
        }

        public override void Dispose()
        {
            base.Dispose();
            _graphGenerator.ClearCaches();
        }
    }
}
