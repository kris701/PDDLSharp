using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.StateSpace.SAS;

namespace PDDLSharp.Toolkit.Planners.Tools
{
    // Operator Relaxed Planning Graph
    public class OperatorRPG
    {
        public List<Layer> GenerateRelaxedPlanningGraph(ISASState state, List<Operator> operators)
        {
            state = state.Copy();
            bool[] covered = new bool[operators.Count];
            List<Layer> layers = new List<Layer>();
            var newLayer = new Layer(new HashSet<Operator>(), state.State);
            for (int i = 0; i < covered.Length; i++)
            {
                if (!covered[i] && state.IsNodeTrue(operators[i]))
                {
                    newLayer.Operators.Add(operators[i]);
                    covered[i] = true;
                }
            }
            layers.Add(newLayer);
            int previousLayer = 0;
            while (!state.IsInGoal())
            {
                // Apply applicable actions to state
                state = state.Copy();
                foreach (var op in layers[previousLayer].Operators)
                    state.ExecuteNode(op);

                if (state.State.Count == layers[previousLayer].Propositions.Count)
                    return new List<Layer>();

                newLayer = new Layer();
                newLayer.Propositions = state.State;
                newLayer.Operators = new HashSet<Operator>(layers[previousLayer].Operators);
                for (int i = 0; i < covered.Length; i++)
                {
                    if (!covered[i] && state.IsNodeTrue(operators[i]))
                    {
                        newLayer.Operators.Add(operators[i]);
                        covered[i] = true;
                    }
                }

                // Error condition: there are no applicable actions at all (most likely means the problem is unsolvable)
                if (newLayer.Operators.Count == 0 && !state.IsInGoal())
                    return new List<Layer>();

                previousLayer++;

                // Add new layer
                layers.Add(newLayer);
            }
            return layers;
        }
    }
}
