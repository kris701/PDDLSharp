using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.PDDL.Overloads
{
    public static class PredicateExpOverloads
    {
        public static bool CanOnlyBeSetToTrue(this PredicateExp self, DomainDecl domain)
        {
            var result = true;

            foreach (var action in domain.Actions)
            {
                var effects = action.Effects.FindNames(self.Name);
                if (effects.Any(x => x.Parent is NotExp))
                    return false;
            }

            return result;
        }

        public static bool CanOnlyBeSetToFalse(this PredicateExp self, DomainDecl domain)
        {
            var result = true;

            foreach (var action in domain.Actions)
            {
                var effects = action.Effects.FindNames(self.Name);
                if (effects.Any(x => x.Parent is not NotExp))
                    return false;
            }

            return result;
        }
    }
}
