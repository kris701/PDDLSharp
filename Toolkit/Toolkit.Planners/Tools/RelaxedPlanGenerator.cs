using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Planners.Exceptions;
using PDDLSharp.Toolkit.StateSpace.SAS;

namespace PDDLSharp.Toolkit.Planners.Tools
{
    public class RelaxedPlanGenerator
    {
        public bool Failed { get; internal set; } = false;
        public SASDecl Declaration { get; set; }
        private OperatorRPG _generator;
        private Dictionary<int, HashSet<Operator>> _opCache;

        public RelaxedPlanGenerator(SASDecl declaration)
        {
            Declaration = declaration;
            _generator = new OperatorRPG();
            _opCache = new Dictionary<int, HashSet<Operator>>();
        }

        public void ClearCaches()
        {
            _opCache.Clear();
            _opCache.EnsureCapacity(0);
            _generator.ClearCaches();
        }

        public HashSet<Operator> GenerateReplaxedPlan(ISASState state, List<Operator> operators)
        {
            var hash = state.GetHashCode();
            if (_opCache.ContainsKey(hash))
                return _opCache[hash];

            Failed = false;
            if (state is not RelaxedSASStateSpace)
                state = new RelaxedSASStateSpace(Declaration, state.State);

            var graphLayers = _generator.GenerateRelaxedPlanningGraph(state, operators);
            if (graphLayers.Count == 0)
            {
                Failed = true;
                return new HashSet<Operator>();
            }
            var selectedOperators = ReconstructPlan(state, graphLayers);

            _opCache.Add(hash, selectedOperators);
            return selectedOperators;
        }

        private HashSet<Operator> ReconstructPlan(ISASState state, List<Layer> graphLayers)
        {
            var selectedOperators = new HashSet<Operator>();
            var m = -1;
            foreach (var fact in Declaration.Goal)
                m = Math.Max(m, FirstLevel(fact, graphLayers));

            if (m == -1)
                throw new RelaxedPlanningGraphException("Relaxed plan graph was not valid!");

            var G = new Dictionary<int, List<Fact>>();
            for (int t = 0; t <= m; t++)
            {
                G.Add(t, new List<Fact>());
                foreach (var fact in Declaration.Goal)
                    if (FirstLevel(fact, graphLayers) == t)
                        G[t].Add(fact);
            }

            for (int t = m; t >= 0; t--)
            {
                foreach (var fact in G[t])
                {
                    foreach (var op in graphLayers[t].Operators)
                    {
                        if (op.Add.Contains(fact))
                        {
                            selectedOperators.Add(op);
                            foreach (var pre in op.Pre)
                            {
                                var newGoal = FirstLevel(pre, graphLayers);
                                if (newGoal == t)
                                    break;
                                G[newGoal].Add(pre);
                            }
                            break;
                        }
                    }
                }
            }

            return selectedOperators;
        }

        private int FirstLevel(Fact fact, List<Layer> layers)
        {
            for (int i = 0; i < layers.Count; i++)
                if (layers[i].Propositions.Contains(fact))
                    return i;
            return -1;
        }
    }
}
