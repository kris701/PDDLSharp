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
            _graphGenerator = new RelaxedPlanGenerator();
        }

        public override ActionPlan Solve(IHeuristic h, IState state)
        {
            Expanded = 0;
            Generated = 0;

            // Initial Operator Subset
            var operators = _graphGenerator.GenerateReplaxedPlan(
                new RelaxedPDDLStateSpace(Declaration),
                GroundedActions
                );

            var closedList = new HashSet<StateMove>();
            var openListRef = new HashSet<StateMove>();
            var openList = new PriorityQueue<StateMove, int>();
            var hValue = h.GetValue(int.MaxValue, state, GroundedActions);
            openList.Enqueue(new StateMove(state, hValue), hValue);

            while (true)
            {
                // Refinement Guard and Refinement
                if (openList.Count == 0)
                    operators = RefineOperators(operators, closedList, openList, openListRef);

                var stateMove = openList.Dequeue();
                if (stateMove.State.IsInGoal())
                {
                    OperatorsUsed = operators.Count;
                    return new ActionPlan(stateMove.Steps, stateMove.Steps.Count);
                }
                openListRef.Remove(stateMove);
                closedList.Add(stateMove);
                foreach (var act in operators)
                {
                    if (stateMove.State.IsNodeTrue(act.Preconditions))
                    {
                        Generated++;
                        var check = stateMove.State.Copy();
                        check.ExecuteNode(act.Effects);
                        var value = h.GetValue(stateMove.hValue, check, GroundedActions);
                        var newMove = new StateMove(check, new List<GroundedAction>(stateMove.Steps) { new GroundedAction(act, act.Parameters.Values) }, value);
                        if (newMove.State.IsInGoal())
                        {
                            OperatorsUsed = operators.Count;
                            return new ActionPlan(newMove.Steps, newMove.Steps.Count);
                        }
                        if (!closedList.Contains(newMove) && !openListRef.Contains(newMove))
                        {
                            openList.Enqueue(newMove, value);
                            openListRef.Add(newMove);
                        }
                    }
                }

                Expanded++;
            }
            throw new Exception("No solution found!");
        }

        private HashSet<ActionDecl> RefineOperators(HashSet<ActionDecl> operators, HashSet<StateMove> closedList, PriorityQueue<StateMove, int> openList, HashSet<StateMove>  openListRef)
        {
            if (closedList.Count == 0)
                return operators;

            bool refinedOperatorsFound = false;
            bool lookForApplicaple = false;
            int smallestHValue = -1;
            // Refinement Step 2
            while (!refinedOperatorsFound && operators.Count != GroundedActions.Count)
            {
                var smallestItem = closedList.Where(x => x.hValue > smallestHValue).MinBy(x => x.hValue);
                if (smallestItem == null)
                {
                    // Refinement step 3
                    if (openList.Count != 0)
                        return operators;

                    if (lookForApplicaple)
                        throw new Exception("??");

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
                        newOperators = GetNewApplicableOperators(smallestHValue, operators, closedList);
                    else
                        newOperators = GetNewRelaxedOperators(smallestHValue, operators, closedList);

                    if (newOperators.Count > 0)
                    {
                        List<StateMove> switchLists = new List<StateMove>(); 
                        foreach(var closed in closedList)
                        {
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
                                closedList.Remove(item);
                                openListRef.Add(item);
                                openList.Enqueue(item, item.hValue);
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
                relaxedPlanOperators.AddRange(
                    _graphGenerator.GenerateReplaxedPlan(
                        new RelaxedPDDLStateSpace(Declaration, item.State.State),
                        GroundedActions
                        ));
            var newOperators = new HashSet<ActionDecl>();
            foreach (var relaxedOperator in relaxedPlanOperators)
                if (!operators.Contains(relaxedOperator))
                    newOperators.Add(relaxedOperator);
            return newOperators;
        }

        private HashSet<ActionDecl> GetNewApplicableOperators(int smallestHValue, HashSet<ActionDecl> operators, HashSet<StateMove> closedList)
        {
            var allSmallest = closedList.Where(x => x.hValue == smallestHValue).ToList();
            var applicableOperators = new HashSet<ActionDecl>();
            foreach (var item in allSmallest)
                foreach (var act in GroundedActions) 
                    if (item.State.IsNodeTrue(act.Preconditions))
                        applicableOperators.Add(act);
            var newOperators = new HashSet<ActionDecl>();
            foreach (var relaxedOperator in applicableOperators)
                if (!operators.Contains(relaxedOperator))
                    newOperators.Add(relaxedOperator);
            return newOperators;
        }
    }
}
