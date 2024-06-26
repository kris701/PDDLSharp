﻿using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Tools;

namespace PDDLSharp.Toolkits
{
    public class ActionDeclCombiner
    {
        public ActionDecl Combine(List<ActionDecl> actions)
        {
            if (actions.Count == 0)
                throw new ArgumentException("Cant combine zero actions!");
            var baseAction = actions[0].Copy();

            var basePreAnd = GetExpAsAndExp(baseAction.Preconditions);
            var baseEffAnd = GetExpAsAndExp(baseAction.Effects);

            HashSet<IExp> preconditions = new HashSet<IExp>();
            preconditions.AddRange(basePreAnd.Children.ToHashSet());
            HashSet<IExp> effects = new HashSet<IExp>();
            effects.AddRange(baseEffAnd.Children.ToHashSet());

            foreach (var action in actions.Skip(1))
            {
                // Add to name
                baseAction.Name = $"{baseAction.Name}-{action.Name}";

                // Add all parameters
                baseAction.Parameters.Values.AddRange(action.Parameters.Values);

                // Combine preconditions
                var preAnd = GetExpAsAndExp(action.Preconditions);
                foreach (var pre in preAnd.Children)
                {
                    if (!effects.Contains(pre) && !preconditions.Contains(pre))
                        preconditions.Add(pre);
                }

                // Combine effects
                var effAnd = GetExpAsAndExp(action.Effects);
                foreach (var pre in effAnd.Children)
                {
                    if (pre is NotExp not)
                        effects.Remove(not.Child);
                    else
                        effects.Remove(new NotExp(pre));
                    if (!effects.Contains(pre))
                        effects.Add(pre);
                }

            }

            basePreAnd.Children = preconditions.ToList();
            baseEffAnd.Children = RemoveUnneededSideEffects(effects, preconditions).ToList();

            baseAction.Preconditions = basePreAnd;
            baseAction.Effects = baseEffAnd;
            baseAction.Parameters.Values = GetReferencesParameters(baseAction);

            return baseAction;
        }

        private HashSet<IExp> RemoveUnneededSideEffects(HashSet<IExp> effects, HashSet<IExp> preconditions)
        {
            HashSet<IExp> newEffects = new HashSet<IExp>();
            foreach (var effect in effects)
            {
                if (effect is NotExp not)
                {
                    if (preconditions.Contains(not.Child))
                        newEffects.Add(effect);
                }
                else if (!preconditions.Contains(effect))
                    newEffects.Add(effect);
            }
            return newEffects;
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
            baseAction.Parameters.Values = baseAction.Parameters.Values.DistinctBy(x => x.Name).ToList();
            baseAction.Parameters.Values.RemoveAll(x => !allRefs.Any(y => y.Name == x.Name));
            return baseAction.Parameters.Values;
        }
    }
}
