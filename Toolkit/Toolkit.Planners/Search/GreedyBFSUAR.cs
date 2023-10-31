using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Planners.Exceptions;
using PDDLSharp.Toolkit.Planners.Tools;
using PDDLSharp.Toolkit.StateSpace;
using PDDLSharp.Toolkit.StateSpace.SAS;
using PDDLSharp.Tools;

namespace PDDLSharp.Toolkit.Planners.Search
{
    // Greedy Search with Under-Approximation Refinement
    public class GreedyBFSUAR : BaseSearch
    {
        public int OperatorsUsed { get; set; }

        private RelaxedPlanGenerator _graphGenerator;
        public GreedyBFSUAR(PDDLDecl decl, IHeuristic heuristic) : base(decl, heuristic)
        {
            _graphGenerator = new RelaxedPlanGenerator(decl);
        }

        internal override ActionPlan Solve(IHeuristic h, IState<Fact, Operator> state)
        {
            // Initial Operator Subset
            var operators = GetInitialOperators();

            int best = 0;
            int current = 0;

            while (!Aborted)
            {
                // Refinement Guards
                if (_openList.Count == 0 || current > best)
                    operators = RefineOperators(operators);

                var stateMove = ExpandBestState();
                if (stateMove.State.IsInGoal())
                {
                    OperatorsUsed = operators.Count;
                    return new ActionPlan(stateMove.Steps);
                }
                best = stateMove.hValue;
                current = int.MaxValue;
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
                            var value = h.GetValue(stateMove, newMove.State, Operators);
                            if (value < current)
                                current = value;
                            newMove.Steps = new List<GroundedAction>(stateMove.Steps) { GenerateFromOp(op) };
                            newMove.hValue = value;
                            _openList.Enqueue(newMove, value);
                        }
                    }
                }
            }
            throw new NoSolutionFoundException();
        }

        private HashSet<Operator> GetInitialOperators()
        {
            var operators = _graphGenerator.GenerateReplaxedPlan(
                new RelaxedSASStateSpace(Declaration),
                Operators
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
            while (!refinedOperatorsFound && operators.Count != Operators.Count)
            {
                var smallestItem = _closedList.Where(x => x.hValue > smallestHValue).MinBy(x => x.hValue);
                if (smallestItem == null)
                {
                    // Refinement step 3
                    if (_openList.Count != 0)
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
                        newOperators = GetNewApplicableOperators(smallestHValue, operators, _closedList);
                    else
                        newOperators = GetNewRelaxedOperators(smallestHValue, operators, _closedList);

                    if (newOperators.Count > 0)
                    {
                        List<StateMove> switchLists = new List<StateMove>();
                        foreach (var closed in _closedList)
                        {
                            if (closed.hValue != smallestItem.hValue)
                                continue;
                            foreach (var newOperator in newOperators)
                            {
                                if (closed.State.IsNodeTrue(newOperator))
                                {
                                    switchLists.Add(closed);
                                    break;
                                }
                            }
                        }

                        if (switchLists.Count > 0)
                        {
                            foreach (var item in switchLists)
                            {
                                _closedList.Remove(item);
                                _openList.Enqueue(item, item.hValue);
                            }

                            operators.AddRange(newOperators);
                            refinedOperatorsFound = true;
                        }
                        else
                        {
                            // Refinement Step 4
                            smallestHValue = -1;
                            lookForApplicaple = true;
                        }
                    }
                }
            }
            return operators;
        }

        private HashSet<Operator> GetNewRelaxedOperators(int smallestHValue, HashSet<Operator> operators, HashSet<StateMove> closedList)
        {
            var allSmallest = closedList.Where(x => x.hValue == smallestHValue).ToList();
            var relaxedPlanOperators = new HashSet<Operator>();
            foreach (var item in allSmallest)
            {
                var newOps = _graphGenerator.GenerateReplaxedPlan(
                        item.State,
                        Operators
                        );
                if (!_graphGenerator.Failed)
                    relaxedPlanOperators.AddRange(newOps);
            }
            return relaxedPlanOperators.Except(operators).ToHashSet();
        }

        private Dictionary<int, HashSet<Operator>> _applicableCache = new Dictionary<int, HashSet<Operator>>();
        private HashSet<Operator> GetNewApplicableOperators(int smallestHValue, HashSet<Operator> operators, HashSet<StateMove> closedList)
        {
            var allSmallest = closedList.Where(x => x.hValue == smallestHValue).ToList();
            var applicableOperators = new HashSet<Operator>();
            foreach (var item in allSmallest)
            {
                var hash = item.GetHashCode();
                if (_applicableCache.ContainsKey(hash))
                    applicableOperators.AddRange(_applicableCache[hash]);
                else
                {
                    _applicableCache.Add(hash, new HashSet<Operator>());
                    foreach (var op in Operators)
                    {
                        if (item.State.IsNodeTrue(op))
                        {
                            applicableOperators.Add(op);
                            _applicableCache[hash].Add(op);
                        }
                    }
                }
            }
            return applicableOperators.Except(operators).ToHashSet();
        }

        public override void Dispose()
        {
            base.Dispose();
            _graphGenerator.ClearCaches();
        }
    }
}
