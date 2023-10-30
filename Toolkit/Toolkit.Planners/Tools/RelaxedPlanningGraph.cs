using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Toolkit.StateSpace;

namespace PDDLSharp.Toolkit.Planners.Tools
{
    public class RelaxedPlanningGraph
    {
        // Cache, from the hash of the previous state, that then links to the next layer
        private Dictionary<int, Layer> _layerCache = new Dictionary<int, Layer>();
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
                // Take from cache if it exists
                var hash = state.GetHashCode();
                if (_layerCache.ContainsKey(hash))
                {
                    state.State = _layerCache[hash].Propositions;
                    foreach (var item in _coveredCache[hash])
                        covered[item] = true;

                    // Error condition: there where actions executed, but nothing happened from them
                    if (layers[previousLayer++].Propositions.Count == _layerCache[hash].Propositions.Count)
                        return new List<Layer>();

                    layers.Add(_layerCache[hash]);
                }
                else
                {
                    var newLayer = new Layer();
                    var newCovers = new List<int>();
                    // Find applicable actions
                    for (int i = 0; i < covered.Length; i++)
                    {
                        if (!covered[i])
                        {
                            if (state.IsNodeTrue(groundedActions[i].Preconditions))
                            {
                                newLayer.Actions.Add(groundedActions[i]);
                                covered[i] = true;
                                newCovers.Add(i);
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
                    _layerCache.Add(hash, newLayer);
                    _coveredCache.Add(hash, newCovers);
                }
            }
            return layers;
        }
    }
}
