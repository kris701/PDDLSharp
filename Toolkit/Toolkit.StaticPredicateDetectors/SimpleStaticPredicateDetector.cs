using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.StaticPredicateDetectors
{
    public class SimpleStaticPredicateDetector : IStaticPredicateDetectors
    {
        public List<PredicateExp> FindStaticPredicates(PDDLDecl decl)
        {
            List<PredicateExp> statics = new List<PredicateExp>();

            if (decl.Domain != null && decl.Domain.Predicates != null)
            {
                var allPredicates = decl.Domain.Predicates.FindTypes<PredicateExp>();
                foreach(var action in decl.Domain.Actions)
                {
                    var effects = action.Effects.FindTypes<PredicateExp>();
                    allPredicates.RemoveAll(x => effects.Any(y => y.Name == x.Name));
                }
                foreach (var pred in allPredicates)
                    statics.Add(pred.Copy());
            }

            return statics;
        }
    }
}
