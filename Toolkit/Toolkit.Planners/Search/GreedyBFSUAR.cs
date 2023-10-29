using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.Plans;
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
                // Refinement Guard and Refinement
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
                        var check = GenerateNewState(stateMove.State, act);
                        var newMove = new StateMove(check);
                        if (newMove.State.IsInGoal())
                        {
                            OperatorsUsed = operators.Count;
                            return new ActionPlan(new List<GroundedAction>(stateMove.Steps) { new GroundedAction(act, act.Parameters.Values) });
                        }
                        if (!_closedList.Contains(newMove) && !_openList.Contains(newMove))
                        {
                            var value = h.GetValue(stateMove.hValue, check, GroundedActions);
                            if (value < current)
                                current = value;
                            newMove.Steps = new List<GroundedAction>(stateMove.Steps) { new GroundedAction(act, act.Parameters.Values) };
                            newMove.hValue = value;
                            _openList.Enqueue(newMove, value);
                        }
                    }
                }
            }
            throw new Exception("No solution found!");
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
                        throw new Exception("No solution found!");

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
