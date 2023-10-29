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
    public class RelaxedPlanningGraph
    {
        private Dictionary<int, Layer> _stateCache = new Dictionary<int, Layer>();
        private Dictionary<int, List<int>> _coveredCache = new Dictionary<int, List<int>>();
        public List<Layer> GenerateRelaxedPlanningGraph(IState state, List<ActionDecl> groundedActions)
        {
            state = state.Copy();
            bool[] covered = new bool[groundedActions.Count];
            List<Layer> layers = new List<Layer>();
            layers.Add(new Layer(new HashSet<ActionDecl>(), state.State));
            int previousLayer = 0;
            while (!state.IsInGoal())
            {
                var hash = state.GetHashCode();
                if (_stateCache.ContainsKey(hash))
                {
                    state.State = _stateCache[hash].Propositions;
                    foreach (var item in _coveredCache[hash])
                        covered[item] = true;
                    layers.Add(_stateCache[hash]);
                }
                else
                {
                    var newLayer = new Layer();
                    _coveredCache.Add(hash, new List<int>());
                    // Find applicable actions
                    for (int i = 0; i < covered.Length; i++)
                    {
                        if (!covered[i])
                        {
                            if (state.IsNodeTrue(groundedActions[i].Preconditions))
                            {
                                newLayer.Actions.Add(groundedActions[i]);
                                covered[i] = true;
                                _coveredCache[hash].Add(i);
                            }
                        }
                    }
                    // Error condition: there are no applicable actions at all (most likely means the problem is unsolvable)
                    if (newLayer.Actions.Count == 0)
                        return new List<Layer>();

                    // Apply applicable actions to state
                    state = state.Copy();
                    foreach (var act in newLayer.Actions)
                        state.ExecuteNode(act.Effects);
                    newLayer.Propositions = state.State;

                    // Error condition: there where actions executed, but nothing happened from them
                    if (layers[previousLayer++].Propositions.Count == newLayer.Propositions.Count)
                        return new List<Layer>();

                    // Add new layer
                    layers.Add(newLayer);
                    _stateCache.Add(hash, newLayer);
                }
            }
            return layers;
        }
    }
}
