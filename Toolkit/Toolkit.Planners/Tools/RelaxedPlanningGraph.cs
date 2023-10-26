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
        public static List<Layer> GenerateRelaxedPlanningGraph(IState state, HashSet<ActionDecl> groundedActions)
        {
            state = state.Copy();
            ActionDecl[] copyActs = new ActionDecl[groundedActions.Count];
            groundedActions.CopyTo(copyActs);
            groundedActions = copyActs.ToHashSet();
            List<Layer> layers = new List<Layer>();
            layers.Add(new Layer(new HashSet<ActionDecl>(), state.State));
            while (!state.IsInGoal())
            {
                // Find applicable actions
                var newLayer = new Layer();
                foreach (var act in groundedActions)
                    if (state.IsNodeTrue(act.Preconditions))
                        newLayer.Actions.Add(act);
                // Error condition: there are no applicable actions at all (most likely means the problem is unsolvable)
                if (newLayer.Actions.Count == 0)
                    throw new ArgumentException("No applicable actions found!");

                // Apply applicable actions to state
                state = state.Copy();
                foreach (var act in newLayer.Actions)
                {
                    state.ExecuteNode(act.Effects);
                    groundedActions.Remove(act);
                }
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
