using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.Plans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

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
            var getTopNBlocks = GetTopN(occurenceCount, amount);
            var instances = GetActionInstancesFromGrounded(getTopNBlocks);
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

        private List<ActionSequence> GetTopN(Dictionary<ActionSequence, int> occurenceCount, int n)
        {
            List<ActionSequence> topn = new List<ActionSequence>();

            if (occurenceCount.Keys.Count > 0)
            {
                occurenceCount = occurenceCount.Where(x => x.Value > 1).ToDictionary(pair => pair.Key, pair => pair.Value);
                occurenceCount.OrderBy(x => x.Value);
                n = Math.Min(n, occurenceCount.Count);
                foreach (var occurence in occurenceCount.Keys.Take(n))
                    topn.Add(occurence);
            }

            return topn;
        }

        private List<List<ActionDecl>> GetActionInstancesFromGrounded(List<ActionSequence> actionBlocks)
        {
            var instances = new List<List<ActionDecl>>();
            var cache = new Dictionary<GroundedAction, ActionDecl>();
            foreach (var block in actionBlocks)
            {
                var instance = new List<ActionDecl>();
                foreach(var action in block.Actions)
                {
                    if (cache.ContainsKey(action))
                    {
                        instance.Add(cache[action]);
                    }
                    else
                    {
                        ActionDecl target = Declaration.Domain.Actions.First(x => x.Name == action.ActionName).Copy();
                        var allNames = target.FindTypes<NameExp>();
                        for (int i = 0; i < action.Arguments.Count; i++)
                        {
                            var allRefs = allNames.Where(x => x.Name == target.Parameters.Values[i].Name).ToList();
                            foreach (var referene in allRefs)
                                referene.Name = $"?{action.Arguments[i].Name}";
                        }
                        cache.Add(action, target);
                        instance.Add(target);
                    }
                }
                instances.Add(instance);
            }
            return instances;
        }

        private List<ActionDecl> CombineBlocks(List<List<ActionDecl>> instances)
        {
            List<ActionDecl> macros = new List<ActionDecl>();
            SimpleActionCombiner combiner = new SimpleActionCombiner();
            foreach (var instance in instances)
                macros.Add(combiner.Combine(instance));
            return macros;
        }
    }
}
