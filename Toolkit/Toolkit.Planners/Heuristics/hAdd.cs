using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models;
using PDDLSharp.Toolkit.StateSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    public class hAdd : IHeuristic
    {
        public PDDLDecl Declaration { get; }
        private RelaxedPlanningGraphs _graphGenerator;

        public hAdd(PDDLDecl declaration)
        {
            Declaration = declaration;
            _graphGenerator = new RelaxedPlanningGraphs();
        }

        public int GetValue(IState state, HashSet<ActionDecl> groundedActions)
        {
            if (state is not RelaxedPDDLStateSpace)
                state = new RelaxedPDDLStateSpace(state.Declaration, state.State);
            else
                state = state.Copy();

            int cost = 0;
            var goalFacts = GetGoalFacts(Declaration.Problem);
            foreach (var fact in goalFacts)
                cost += GetCostToAchiveFact(state, fact, groundedActions);
            return cost;
        }

        private HashSet<PredicateExp> GetGoalFacts(ProblemDecl problem)
        {
            var returnSet = new HashSet<PredicateExp>();

            if (problem.Goal != null)
            {
                var allPreds = problem.Goal.GoalExp.FindTypes<PredicateExp>();
                foreach(var pred in allPreds)
                    if (pred.Parent is not NotExp)
                        returnSet.Add(pred);
            }

            return returnSet;
        }

        private int GetCostToAchiveFact(IState state, PredicateExp pred, HashSet<ActionDecl> groundedActions)
        {
            if (state.Contains(pred))
                return 0;

            var targetActions = new HashSet<ActionDecl>();
            foreach(var op in groundedActions)
                if (op.Effects is AndExp and && and.Children.Contains(pred))
                    targetActions.Add(op);
            if (targetActions.Count == 0)
                return int.MaxValue;

            int min = int.MaxValue;
            foreach(var op in targetActions)
            {
                var value = 1 + GetCostOfOperatorPreconditions(state, op, groundedActions);
                if (value < min)
                    min = value;
            }
            return min;
        }

        private int GetCostOfOperatorPreconditions(IState state, ActionDecl op, HashSet<ActionDecl> groundedActions)
        {
            var allPrecons = op.Preconditions.FindTypes<PredicateExp>();
            allPrecons.RemoveAll(x => x.Parent is NotExp);
            int cost = 0;
            foreach (var recon in allPrecons)
                cost += GetCostToAchiveFact(state, recon, groundedActions);
            return cost;
        }
    }
}
