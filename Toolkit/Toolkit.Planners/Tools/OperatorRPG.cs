using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.StateSpace.SAS;

namespace PDDLSharp.Toolkit.Planners.Tools
{
    // Operator Relaxed Planning Graph
    public class OperatorRPG
    {
        public void ClearCaches()
        {
            _layerCache.Clear();
            _layerCache.EnsureCapacity(0);
            _coveredCache.Clear();
            _coveredCache.EnsureCapacity(0);
        }

        // Cache, from the hash of the previous state, that then links to the next layer
        private Dictionary<int, Layer> _layerCache = new Dictionary<int, Layer>();
        private Dictionary<int, List<int>> _coveredCache = new Dictionary<int, List<int>>();
        public List<Layer> GenerateRelaxedPlanningGraph(ISASState state, List<Operator> operators)
        {
            state = state.Copy();
            bool[] covered = new bool[operators.Count];
            List<Layer> layers = new List<Layer>();
            var newLayer = new Layer(new HashSet<Operator>(), state.State);
            for (int i = 0; i < covered.Length; i++)
            {
                if (!covered[i])
                {
                    if (state.IsNodeTrue(operators[i]))
                    {
                        newLayer.Operators.Add(operators[i]);
                        covered[i] = true;
                    }
                }
            }
            layers.Add(newLayer);
            int previousLayer = 0;
            while (!state.IsInGoal())
            {
                // Take from cache if it exists
                //var hash = state.GetHashCode() ^ operators.Count;
                //if (_layerCache.ContainsKey(hash))
                //{
                //    state.State = _layerCache[hash].Propositions;
                //    foreach (var item in _coveredCache[hash])
                //        covered[item] = true;

                //    // Error condition: there where actions executed, but nothing happened from them
                //    if (layers[previousLayer++].Propositions.Count == _layerCache[hash].Propositions.Count)
                //        return new List<Layer>();

                //    layers.Add(_layerCache[hash]);
                //}
                //else
                //{

                // Apply applicable actions to state
                state = state.Copy();
                foreach (var op in layers[previousLayer].Operators)
                    state.ExecuteNode(op);

                if (state.State.Count == layers[previousLayer].Propositions.Count)
                    return new List<Layer>();

                newLayer = new Layer();
                newLayer.Propositions = state.State;

                //var newCovers = new List<int>();
                // Find applicable actions
                for (int i = 0; i < covered.Length; i++)
                {
                    //if (!covered[i])
                    {
                        if (state.IsNodeTrue(operators[i]))
                        {
                            newLayer.Operators.Add(operators[i]);
                            //covered[i] = true;
                            //newCovers.Add(i);
                        }
                    }
                }

                // Error condition: there are no applicable actions at all (most likely means the problem is unsolvable)
                if (newLayer.Operators.Count == 0 && !state.IsInGoal())
                        return new List<Layer>();

                    //// Apply applicable actions to state
                    //state = state.Copy();
                    //foreach (var op in newLayer.Operators)
                    //    state.ExecuteNode(op);
                    //newLayer.Propositions = state.State;

                    //// Error condition: there where actions executed, but nothing happened from them
                    //if (layers[previousLayer].Propositions.Count == newLayer.Propositions.Count &&
                    //    layers[previousLayer].Operators.Count == newLayer.Operators.Count)
                    //    return new List<Layer>();
                previousLayer++;

                    // Add new layer
                    layers.Add(newLayer);
                //    _layerCache.Add(hash, newLayer);
                //    _coveredCache.Add(hash, newCovers);
                //}
            }
            return layers;
        }
    }
}
