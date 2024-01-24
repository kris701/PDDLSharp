using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Toolkit.MacroGenerators.Models;
using PDDLSharp.Toolkit.MacroGenerators.Tools;

namespace PDDLSharp.Toolkit.MacroGenerators
{
    public class SequentialMacroGenerator : IMacroGenerator<List<ActionPlan>>
    {
        public PDDLDecl Declaration { get; }

        public SequentialMacroGenerator(PDDLDecl declaration)
        {
            Declaration = declaration;
        }

        public List<ActionDecl> FindMacros(List<ActionPlan> from, int amount = int.MaxValue)
        {
            var actionBlocks = GetBlocks(from);
            var occurenceCount = GetOccurenceDict(actionBlocks);
            var instances = GetActionInstancesFromGrounded(occurenceCount, amount);
            var macros = CombineBlocks(instances);

            return macros;
        }

        private List<ActionSequence> GetBlocks(List<ActionPlan> from)
        {
            var actionBlocks = new List<ActionSequence>();
            foreach (var plan in from)
                for (int blockSize = plan.Plan.Count; blockSize > 1; blockSize--)
                    for (int offset = 0; offset + blockSize <= plan.Plan.Count; offset++)
                        actionBlocks.Add(new ActionSequence(plan.Plan.GetRange(offset, blockSize)));
            return actionBlocks;
        }

        private Dictionary<ActionSequence, int> GetOccurenceDict(List<ActionSequence> actionBlocks)
        {
            Dictionary<ActionSequence, int> occurenceCount = new Dictionary<ActionSequence, int>();
            foreach (var block in actionBlocks)
            {
                if (occurenceCount.ContainsKey(block))
                    occurenceCount[block]++;
                else
                    occurenceCount.Add(block, 1);
            }
            return occurenceCount;
        }

        private List<List<ActionDecl>> GetActionInstancesFromGrounded(Dictionary<ActionSequence, int> occurenceCount, int n)
        {
            var instances = new List<List<ActionDecl>>();

            if (occurenceCount.Count > 0)
            {
                // Select only occurences that is larger than 1.
                occurenceCount = occurenceCount.Where(x => x.Value > 1).ToDictionary(pair => pair.Key, pair => pair.Value);
                occurenceCount = occurenceCount.OrderBy(x => x.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
                n = Math.Min(n, occurenceCount.Count);

                var cache = new Dictionary<GroundedAction, ActionDecl>();
                foreach (var occurence in occurenceCount.Keys)
                {
                    var instance = new List<ActionDecl>();
                    foreach (var action in occurence.Actions)
                    {
                        if (cache.ContainsKey(action))
                        {
                            instance.Add(cache[action]);
                        }
                        else
                        {
                            ActionDecl target = GenerateActionInstance(action);
                            cache.Add(action, target);
                            instance.Add(target);
                        }
                    }
                    if (IsActionChainValid(instance))
                        instances.Add(instance);
                    if (instances.Count >= n)
                        break;
                }
            }
            return instances;
        }

        private ActionDecl GenerateActionInstance(GroundedAction action)
        {
            ActionDecl target = Declaration.Domain.Actions.First(x => x.Name == action.ActionName).Copy();
            var allNames = target.FindTypes<NameExp>();
            for (int i = 0; i < action.Arguments.Count; i++)
            {
                var allRefs = allNames.Where(x => x.Name == target.Parameters.Values[i].Name).ToList();
                foreach (var referene in allRefs)
                    referene.Name = $"?{action.Arguments[i].Name}";
            }
            return target;
        }

        private bool IsActionChainValid(List<ActionDecl> actions)
        {
            var covered = new bool[actions.Count];
            Dictionary<int, HashSet<IExp>> effCache = new Dictionary<int, HashSet<IExp>>();
            Dictionary<int, HashSet<IExp>> preCache = new Dictionary<int, HashSet<IExp>>();
            for (int current = 0; current < actions.Count; current++)
            {
                if (!covered[current])
                {
                    var currentEffect = GetOrCreateCacheInstance(current, actions[current].Effects, effCache);
                    for (int seek = current + 1; seek < actions.Count; seek++)
                    {
                        var seekPreconditions = GetOrCreateCacheInstance(seek, actions[seek].Preconditions, preCache);
                        if (seekPreconditions.Any(currentEffect.Contains))
                        {
                            covered[current] = true;
                            covered[seek] = true;
                        }
                    }
                    if (!covered[current])
                        return false;
                }
            }
            bool isCovered = covered.All(x => x == true);
            return isCovered;
        }

        private HashSet<IExp> GetOrCreateCacheInstance(int index, IExp target, Dictionary<int, HashSet<IExp>> cache)
        {
            if (cache.ContainsKey(index))
                return cache[index];
            var effect = GetExpAsAndExp(target).Children.ToHashSet();
            cache.Add(index, effect);
            return effect;
        }

        private AndExp GetExpAsAndExp(IExp from)
        {
            if (from is AndExp and)
                return and;
            return new AndExp(new List<IExp>() { from }); ;
        }

        private List<ActionDecl> CombineBlocks(List<List<ActionDecl>> instances)
        {
            List<ActionDecl> macros = new List<ActionDecl>();
            ActionDeclCombiner combiner = new ActionDeclCombiner();
            foreach (var instance in instances)
                macros.Add(combiner.Combine(instance));
            return macros;
        }
    }
}
