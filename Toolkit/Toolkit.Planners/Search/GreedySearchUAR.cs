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

namespace PDDLSharp.Toolkit.Planners.Search
{
    // Greedy Search with Under-Approximation Refinement
    public class GreedySearchUAR : IPlanner
    {
        public DomainDecl Domain { get; }
        public ProblemDecl Problem { get; }
        public HashSet<ActionDecl> GroundedActions { get; set; }
        public int Generated { get; internal set; }
        public int Expanded { get; internal set; }

        private bool _preprocessed = false;
        private RelaxedPlanningGraphs _graphGenerator;

        public GreedySearchUAR(DomainDecl domain, ProblemDecl problem)
        {
            Domain = domain;
            Problem = problem;
            GroundedActions = new HashSet<ActionDecl>();
            _graphGenerator = new RelaxedPlanningGraphs();
        }

        public void PreProcess()
        {
            if (_preprocessed)
                return;
            IGrounder<ActionDecl> grounder = new ActionGrounder(new PDDLDecl(Domain, Problem));
            GroundedActions = new HashSet<ActionDecl>();
            foreach (var action in Domain.Actions)
                GroundedActions.AddRange(grounder.Ground(action).ToHashSet());
            _preprocessed = true;
        }

        public ActionPlan Solve(IHeuristic h)
        {
            IState state = new PDDLStateSpace(new PDDLDecl(Domain, Problem));
            return Solve(h, state);
        }

        public ActionPlan Solve(IHeuristic h, IState state)
        {
            Expanded = 0;
            Generated = 0;

            // Initial Operator Subset
            var operators = _graphGenerator.GenerateReplaxedPlan(
                new RelaxedPDDLStateSpace(new PDDLDecl(Domain, Problem)),
                GroundedActions
                );

            HashSet<StateMove> closedList = new HashSet<StateMove>();
            Queue<StateMove> openList = new Queue<StateMove>();
            openList.Enqueue(new StateMove(state, h.GetValue(state, GroundedActions)));

            while (true)
            {
                // Refinement Guard and Refinement
                if (openList.Count == 0)
                    operators = RefineOperators(operators, closedList, openList);

                var stateMove = openList.Dequeue();

                foreach (var act in operators)
                {
                    if (stateMove.State.IsNodeTrue(act.Preconditions))
                    {
                        Expanded++;
                        var check = stateMove.State.Copy();
                        check.ExecuteNode(act.Effects);
                        var value = h.GetValue(check, GroundedActions);
                        var newMove = new StateMove(check, new List<GroundedAction>(stateMove.Steps) { new GroundedAction(act, act.Parameters.Values) }, value);
                        if (!closedList.Contains(newMove) && !openList.Contains(newMove))
                        {
                            if (value < stateMove.hValue)
                            {
                                Generated++;
                                openList.Enqueue(newMove);
                                if (check.IsInGoal())
                                    return new ActionPlan(newMove.Steps, stateMove.Steps.Count);
                            }
                            closedList.Add(newMove);
                        }
                    }
                }
            }
            throw new Exception("No solution found!");
        }

        private HashSet<ActionDecl> RefineOperators(HashSet<ActionDecl> operators, HashSet<StateMove> closedList, Queue<StateMove> openList)
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
                        foreach(var item in switchLists)
                        {
                            closedList.Remove(item);
                            openList.Enqueue(item);
                        }

                        operators.AddRange(newOperators);
                        refinedOperatorsFound = true;
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
                        new RelaxedPDDLStateSpace(new PDDLDecl(Domain, Problem), item.State.State),
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
            {
                foreach (var act in GroundedActions) {
                    if (item.State.IsNodeTrue(act.Preconditions))
                    {
                        applicableOperators.Add(act);
                        break;
                    }
                }
            }
            var newOperators = new HashSet<ActionDecl>();
            foreach (var relaxedOperator in applicableOperators)
                if (!operators.Contains(relaxedOperator))
                    newOperators.Add(relaxedOperator);
            return newOperators;
        }
    }
}
