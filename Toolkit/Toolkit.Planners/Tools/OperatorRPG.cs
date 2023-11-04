using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.StateSpace.SAS;

namespace PDDLSharp.Toolkit.Planners.Tools
{
    public class OperatorRPG : BaseRPG
    {
        public bool Failed { get; internal set; } = false;
        public SASDecl Declaration { get; set; }

        public OperatorRPG(SASDecl declaration)
        {
            Declaration = declaration;
        }

        public List<Operator> GenerateReplaxedPlan(ISASState state, List<Operator> operators)
        {
            Failed = false;
            if (state is not RelaxedSASStateSpace)
                state = new RelaxedSASStateSpace(Declaration, state.State);

            var graphLayers = GenerateRelaxedPlanningGraph(state, operators);
            if (graphLayers.Count == 0)
            {
                Failed = true;
                return new List<Operator>();
            }
            var selectedOperators = ReconstructPlan2(graphLayers);

            return selectedOperators;
        }

        // Hoffman & Nebel 2001, Figure 2
        private List<Operator> ReconstructPlan2(List<Layer> graphLayers)
        {
            var selectedOperators = new List<Operator>();
            var G = new Dictionary<int, HashSet<Fact>>();
            var trues = new Dictionary<int, HashSet<Fact>>();
            var m = -1;
            foreach (var fact in Declaration.Goal)
                m = Math.Max(m, FirstLevel(fact, graphLayers));

            G.Add(0, new HashSet<Fact>());
            trues.Add(0, new HashSet<Fact>());
            for (int t = 1; t <= m; t++)
            {
                G.Add(t, new HashSet<Fact>());
                trues.Add(t, new HashSet<Fact>());
                foreach (var fact in Declaration.Goal)
                    if (FirstLevel(fact, graphLayers) == t)
                        G[t].Add(fact);
            }

            for (int i = m; i > 0; i--)
            {
                foreach (var fact in G[i])
                {
                    if (trues[i].Contains(fact))
                        continue;

                    var options = new PriorityQueue<Operator, int>();
                    foreach (var op in graphLayers[i - 1].Operators)
                        if (op.AddRef.Contains(fact.ID))
                            options.Enqueue(op, Difficulty(op, graphLayers));

                    if (options.Count > 0)
                    {
                        var best = options.Dequeue();
                        selectedOperators.Add(best);
                        foreach (var pre in best.Pre)
                        {
                            var targetLayer = FirstLevel(pre, graphLayers);
                            if (targetLayer == i || trues[i - 1].Contains(pre))
                                continue;
                            G[targetLayer].Add(pre);
                        }

                        foreach (var add in best.Add)
                        {
                            trues[i - 1].Add(add);
                            trues[i].Add(add);
                        }
                    }
                }
            }

            return selectedOperators;
        }

        // Hoffman & Nebel 2001, Equation 4
        private int Difficulty(Operator op, List<Layer> graphLayers)
        {
            int diff = int.MaxValue;
            foreach (var pre in op.Pre)
                diff = Math.Min(diff, FirstLevel(pre, graphLayers));
            return diff;
        }

        private int FirstLevel(Fact fact, List<Layer> layers)
        {
            for (int i = 0; i < layers.Count; i++)
                if (layers[i].Propositions.Contains(fact))
                    return i;
            throw new Exception();
        }

        public List<Layer> GenerateRelaxedPlanningGraph(ISASState state, List<Operator> operators)
        {
            state = state.Copy();
            bool[] covered = new bool[operators.Count];
            List<Layer> layers = new List<Layer>();
            var newLayer = new Layer(
                GetNewApplicableOperators(state, new List<Operator>(), operators, covered),
                state.State);
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

                newLayer = new Layer(
                    GetNewApplicableOperators(state, layers[previousLayer].Operators, operators, covered),
                    state.State);

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
