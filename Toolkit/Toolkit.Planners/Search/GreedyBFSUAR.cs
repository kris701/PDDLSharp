using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models;
using PDDLSharp.Toolkit.Grounders;
using PDDLSharp.Toolkit.StateSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Toolkit.Planners.Heuristics;
using PDDLSharp.Models.PDDL.Expressions;
using System.Xml.Linq;
using PDDLSharp.Tools;
using PDDLSharp.Toolkit.Planners.Tools;
using System.Runtime.Intrinsics.X86;
using PDDLSharp.Toolkit.Planners.Exceptions;
using PDDLSharp.Models.FastDownward.Plans;

namespace PDDLSharp.Toolkit.Planners.Search
{
    // Greedy Search with Under-Approximation Refinement
    public class GreedyBFSUAR : BaseSearch
    {
        public int OperatorsUsed { get; set; }

        private RelaxedPlanGenerator _graphGenerator;

        public GreedyBFSUAR(PDDLDecl decl) : base(decl)
        {
            _graphGenerator = new RelaxedPlanGenerator(decl);
        }

        internal override ActionPlan Solve(IHeuristic h, IState state)
        {
            // Initial Operator Subset
            var operators = GetInitialOperators();

            int best = 0;
            int current = 0;

            while (!_abort)
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
                foreach (var act in operators)
                {
                    if (_abort) break;
                    if (stateMove.State.IsNodeTrue(act.Preconditions))
                    {
                        var newMove = new StateMove(GenerateNewState(stateMove.State, act));
                        if (newMove.State.IsInGoal())
                        {
                            OperatorsUsed = operators.Count;
                            return new ActionPlan(new List<GroundedAction>(stateMove.Steps) { new GroundedAction(act, act.Parameters.Values) });
                        }
                        if (!_closedList.Contains(newMove) && !_openList.Contains(newMove))
                        {
                            var value = h.GetValue(stateMove, newMove.State, GroundedActions);
                            if (value < current)
                                current = value;
                            newMove.Steps = new List<GroundedAction>(stateMove.Steps) { new GroundedAction(act, act.Parameters.Values) };
                            newMove.hValue = value;
                            _openList.Enqueue(newMove, value);
                        }
                    }
                }
            }
            throw new NoSolutionFoundException();
        }

        private HashSet<ActionDecl> GetInitialOperators()
        {
            var operators = _graphGenerator.GenerateReplaxedPlan(
                new RelaxedPDDLStateSpace(Declaration),
                GroundedActions
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
        private HashSet<ActionDecl> RefineOperators(HashSet<ActionDecl> operators)
        {
            if (_closedList.Count == 0)
                return operators;

            bool refinedOperatorsFound = false;
            bool lookForApplicaple = false;
            int smallestHValue = -1;
            // Refinement Step 2
            while (!refinedOperatorsFound && operators.Count != GroundedActions.Count)
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
                    var newOperators = new HashSet<ActionDecl>();
                    if (lookForApplicaple)
                        newOperators = GetNewApplicableOperators(smallestHValue, operators, _closedList);
                    else
                        newOperators = GetNewRelaxedOperators(smallestHValue, operators, _closedList);

                    if (newOperators.Count > 0)
                    {
                        List<StateMove> switchLists = new List<StateMove>(); 
                        foreach(var closed in _closedList)
                        {
                            if (closed.hValue != smallestItem.hValue)
                                continue;
                            foreach(var newOperator in newOperators)
                            {
                                if (closed.State.IsNodeTrue(newOperator.Preconditions))
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

        private HashSet<ActionDecl> GetNewRelaxedOperators(int smallestHValue, HashSet<ActionDecl> operators, HashSet<StateMove> closedList)
        {
            var allSmallest = closedList.Where(x => x.hValue == smallestHValue).ToList();
            var relaxedPlanOperators = new HashSet<ActionDecl>();
            foreach (var item in allSmallest)
            {
                var newOps = _graphGenerator.GenerateReplaxedPlan(
                        item.State,
                        GroundedActions
                        );
                if (!_graphGenerator.Failed)
                    relaxedPlanOperators.AddRange(newOps);
            }
            return relaxedPlanOperators.Except(operators).ToHashSet();
        }

        private HashSet<ActionDecl> GetNewApplicableOperators(int smallestHValue, HashSet<ActionDecl> operators, HashSet<StateMove> closedList)
        {
            var allSmallest = closedList.Where(x => x.hValue == smallestHValue).ToList();
            var applicableOperators = new HashSet<ActionDecl>();
            foreach (var item in allSmallest)
                foreach (var act in GroundedActions) 
                    if (item.State.IsNodeTrue(act.Preconditions))
                        applicableOperators.Add(act);
            return applicableOperators.Except(operators).ToHashSet();
        }
    }
}
