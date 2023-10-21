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
            var basePreAnd = baseAction.Preconditions as AndExp;
            var baseEffAnd = baseAction.Effects as AndExp;
            if (basePreAnd == null || baseEffAnd == null)
                throw new ArgumentException("Action precondition or effects was not an and node!");
            baseAction.Parameters.Values.RemoveAll(x => baseAction.FindNames(x.Name).Count == 1);

            foreach (var action in actions.Skip(1))
            {
                baseAction.Name = $"{baseAction.Name}-{action.Name}";

                var preAnd = action.Preconditions as AndExp;
                var effAnd = action.Effects as AndExp;
                if (preAnd == null || effAnd == null)
                    throw new ArgumentException("Action precondition or effects was not an and node!");

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

            baseAction.Parameters.Values.RemoveAll(x => baseAction.FindNames(x.Name).Count == 1);

            return baseAction;
        }
    }
}
