using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.MacroGenerators
{
    public class SimpleActionCombiner
    {
        public ActionDecl Combine(List<ActionDecl> actions)
        {
            if (actions.Count == 0)
                throw new ArgumentException("Cant combine no actions!");
            var baseAction = actions[0].Copy();

            var basePreAnd = GetExpAsAndExp(baseAction.Preconditions);
            var baseEffAnd = GetExpAsAndExp(baseAction.Effects);

            foreach (var action in actions.Skip(1))
            {
                baseAction.Name = $"{baseAction.Name}-{action.Name}";

                var preAnd = GetExpAsAndExp(action.Preconditions);
                var effAnd = GetExpAsAndExp(action.Effects);

                foreach(var pre in preAnd.Children)
                    if (!baseEffAnd.Children.Contains(pre) && !basePreAnd.Children.Contains(pre))
                        basePreAnd.Children.Add(pre);

                foreach (var pre in effAnd.Children)
                {
                    if (pre is NotExp not)
                        baseEffAnd.Children.RemoveAll(x => x.GetHashCode() == not.Child.GetHashCode());
                    else
                        baseEffAnd.Children.RemoveAll(x => x is NotExp not && not.Child.GetHashCode() == pre.GetHashCode());
                    if (!baseEffAnd.Children.Contains(pre))
                        baseEffAnd.Children.Add(pre);
                }

                foreach(var argument in action.Parameters.Values)
                    if (!baseAction.Parameters.Values.Contains(argument))
                        if (baseAction.FindNames(argument.Name).Count > 0)
                            baseAction.Parameters.Add(argument);
            }

            baseAction.Parameters.Values = GetReferencesParameters(baseAction);

            return baseAction;
        }

        private AndExp GetExpAsAndExp(IExp from)
        {
            if (from is AndExp and)
                return and;
            return new AndExp(new List<IExp>() { from }); ;
        }

        private List<NameExp> GetReferencesParameters(ActionDecl baseAction)
        {
            var allRefs = baseAction.Preconditions.FindTypes<NameExp>();
            allRefs.AddRange(baseAction.Effects.FindTypes<NameExp>());
            var param = new HashSet<NameExp>();
            foreach (var reference in allRefs)
                if (!param.Contains(reference))
                    param.Add(reference);
            return param.ToList();
        }
    }
}
