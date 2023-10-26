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
                var newLayer = new Layer();
                foreach (var act in groundedActions)
                    if (state.IsNodeTrue(act.Preconditions))
                        newLayer.Actions.Add(act);
                if (newLayer.Actions.Count == 0)
                    throw new Exception("No applicable actions found!");
                state = state.Copy();
                foreach (var act in newLayer.Actions)
                {
                    state.ExecuteNode(act.Effects);
                    groundedActions.Remove(act);
                }
                newLayer.Propositions = state.State;

                if (layers[0].Propositions == newLayer.Propositions)
                    throw new Exception("Relaxed state did not change!");
                layers.Add(newLayer);
            }
            return layers;
        }
    }
}
