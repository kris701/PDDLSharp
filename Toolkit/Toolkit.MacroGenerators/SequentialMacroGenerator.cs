using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Domain;
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
        public int MacroLimit { get; set; } = 10;
        public double SignificanceFactor { get; set; } = 1.5;

        public SequentialMacroGenerator(PDDLDecl declaration)
        {
            Declaration = declaration;
        }

        public List<ActionDecl> FindMacros(List<ActionPlan> from)
        {
            var actionBlocks = GetBlocks(from);
            var occurenceCount = GetOccurenceDict(actionBlocks);
            var significantBlocks = GetSignificants(actionBlocks, occurenceCount);
            var instances = GetActionInstancesFromGrounded(significantBlocks);
            var macros = CombineBlocks(instances);
            var distinctMacros = RemoveDuplicates(macros);

            return distinctMacros;
        }

        private List<List<GroundedAction>> GetBlocks(List<ActionPlan> from)
        {
            var actionBlocks = new List<List<GroundedAction>>();
            foreach (var plan in from)
                for (int blockSize = plan.Plan.Count; blockSize > 1; blockSize--)
                    for (int offset = 0; offset + blockSize <= plan.Plan.Count; offset++)
                        actionBlocks.Add(plan.Plan.GetRange(offset, blockSize));
            return actionBlocks;
        }

        private Dictionary<string, int> GetOccurenceDict(List<List<GroundedAction>> actionBlocks)
        {
            Dictionary<string, int> occurenceCount = new Dictionary<string, int>();
            foreach (var block in actionBlocks)
            {
                var compound = GetCompoundName(block);
                if (occurenceCount.ContainsKey(compound))
                    occurenceCount[compound]++;
                else
                    occurenceCount.Add(compound, 1);
            }
            return occurenceCount;
        }

        private string GetCompoundName(List<GroundedAction> actions)
        {
            string name = "";
            foreach (var action in actions)
                name += action.ActionName;
            return name;
        }

        private List<List<GroundedAction>> GetSignificants(List<List<GroundedAction>> actionBlocks, Dictionary<string, int> occurenceCount)
        {
            List<List<GroundedAction>> significants = new List<List<GroundedAction>>();
            if (occurenceCount.Keys.Count > 0)
            {
                List<string> added = new List<string>();
                double avrCount = occurenceCount.Average(x => x.Value);
                foreach (var block in actionBlocks)
                {
                    var compound = GetCompoundName(block);
                    if (added.Contains(compound))
                        continue;
                    if ((double)occurenceCount[compound] / avrCount > SignificanceFactor)
                    {
                        significants.Add(block);
                        added.Add(compound);
                    }
                    if (added.Count > MacroLimit)
                        break;
                }
            }

            return significants;
        }

        private List<List<ActionDecl>> GetActionInstancesFromGrounded(List<List<GroundedAction>> actionBlocks)
        {
            var instances = new List<List<ActionDecl>>();
            foreach (var block in actionBlocks)
            {
                var instance = new List<ActionDecl>();
                foreach(var action in block)
                {
                    ActionDecl target = Declaration.Domain.Actions.Single(x => x.Name == action.ActionName).Copy();
                    for(int i = 0; i < action.Arguments.Count; i++)
                    {
                        var allRefs = target.FindNames(target.Parameters.Values[i].Name);
                        foreach (var referene in allRefs)
                            referene.Name = $"?{action.Arguments[i].Name}";
                    }
                    instance.Add(target);
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

        private List<ActionDecl> RemoveDuplicates(List<ActionDecl> macros)
        {
            return macros.DistinctBy(x => x.GetHashCode()).ToList();
        }
    }
}
