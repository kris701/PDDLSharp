using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Toolkit.StateSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PDDLSharp.Toolkit.Planners.Tools
{
    public static class RelaxedPlanningGraph
    {
        public static List<Layer> GenerateRelaxedPlanningGraph(IState state, List<ActionDecl> groundedActions)
        {
            state = state.Copy();
            bool[] covered = new bool[groundedActions.Count];
            List<Layer> layers = new List<Layer>();
            layers.Add(new Layer(new HashSet<ActionDecl>(), state.State));
            while (!state.IsInGoal())
            {
                // Find applicable actions
                var newLayer = new Layer();
                for (int i = 0; i < covered.Length; i++)
                {
                    if (!covered[i])
                    {
                        if (state.IsNodeTrue(groundedActions[i].Preconditions))
                        {
                            newLayer.Actions.Add(groundedActions[i]);
                            covered[i] = true;
                        }
                    }
                }
                // Error condition: there are no applicable actions at all (most likely means the problem is unsolvable)
                if (newLayer.Actions.Count == 0)
                    throw new ArgumentException("No applicable actions found!");

                // Apply applicable actions to state
                state = state.Copy();
                foreach (var act in newLayer.Actions)
                    state.ExecuteNode(act.Effects);
                newLayer.Propositions = state.State;

                // Error condition: there where actions executed, but nothing happened from them
                if (layers[0].Propositions == newLayer.Propositions)
                    throw new ArgumentException("Relaxed state did not change!");

                // Add new layer
                layers.Add(newLayer);
            }
            return layers;
        }
    }
}
