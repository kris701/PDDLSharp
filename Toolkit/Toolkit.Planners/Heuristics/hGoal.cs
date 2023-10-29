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
using PDDLSharp.Toolkit.Planners.Search;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    /// <summary>
    /// Based on the <seealso href="https://www.fast-downward.org/Doc/Evaluator">goal count Evaluator</seealso>
    /// </summary>
    public class hGoal : IHeuristic
    {
        public PDDLDecl Declaration { get; }

        private HashSet<PredicateExp> _goalCache = new HashSet<PredicateExp>();

        public hGoal(PDDLDecl declaration)
        {
            Declaration = declaration;
            GenerateGoalFacts(declaration.Problem);
        }

        private void GenerateGoalFacts(ProblemDecl problem)
        {
            var extracted = new HashSet<PredicateExp>();
            if (problem.Goal != null)
            {
                var allPreds = problem.Goal.GoalExp.FindTypes<PredicateExp>();
                foreach (var pred in allPreds)
                    if (pred.Parent is not NotExp)
                        extracted.Add(pred);
            }
            var simplified = new HashSet<PredicateExp>();
            foreach (var fact in extracted)
                simplified.Add(SimplifyPredicate(fact));

            _goalCache = simplified;
        }

        private PredicateExp SimplifyPredicate(PredicateExp pred)
        {
            var newPred = new PredicateExp(pred.Name);
            foreach (var arg in pred.Arguments)
                newPred.Arguments.Add(new NameExp(arg.Name));
            return newPred;
        }

        public int GetValue(StateMove parent, IState state, List<ActionDecl> groundedActions)
        {
            int count = 0;
            foreach(var goal in _goalCache)
                if (state.Contains(goal))
                    count++;
            return _goalCache.Count - count;
        }
    }
}
