using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Translators.StaticPredicateDetectors
{
    public class SimpleStaticPredicateDetector : IStaticPredicateDetectors
    {
        public List<PredicateExp> FindStaticPredicates(PDDLDecl decl)
        {
            List<PredicateExp> statics = new List<PredicateExp>();

            if (decl.Domain != null && decl.Domain.Predicates != null)
            {
                var allPredicates = decl.Domain.Predicates.FindTypes<PredicateExp>();
                foreach (var action in decl.Domain.Actions)
                {
                    var effects = action.Effects.FindTypes<PredicateExp>();
                    allPredicates.RemoveAll(x => effects.Any(y => y.Name == x.Name));
                }
                foreach (var action in decl.Domain.DurativeActions)
                {
                    var effects = action.Effects.FindTypes<PredicateExp>();
                    allPredicates.RemoveAll(x => effects.Any(y => y.Name == x.Name));
                }
                foreach (var action in decl.Domain.Axioms)
                {
                    var effects = action.Implies.FindTypes<PredicateExp>();
                    allPredicates.RemoveAll(x => effects.Any(y => y.Name == x.Name));
                }
                foreach (var pred in allPredicates)
                    statics.Add(pred.Copy());
            }

            return statics;
        }
    }
}
