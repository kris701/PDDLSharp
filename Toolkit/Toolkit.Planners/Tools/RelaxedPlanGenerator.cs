using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Planners.Exceptions;
using PDDLSharp.Toolkit.StateSpace;
using PDDLSharp.Toolkit.StateSpace.SAS;

namespace PDDLSharp.Toolkit.Planners.Tools
{
    public class RelaxedPlanGenerator
    {
        public bool Failed { get; internal set; } = false;
        public PDDLDecl Declaration { get; set; }
        private RelaxedPlanningGraph _generator;
        private Dictionary<int, HashSet<Operator>> _opCache;

        public RelaxedPlanGenerator(PDDLDecl declaration)
        {
            Declaration = declaration;
            _generator = new RelaxedPlanningGraph();
            _opCache = new Dictionary<int, HashSet<Operator>>();
        }

        public HashSet<Operator> GenerateReplaxedPlan(IState<Fact, Operator> state, List<Operator> operators)
        {
            var hash = state.GetHashCode();
            if (_opCache.ContainsKey(hash))
                return _opCache[hash];

            Failed = false;
            if (state is not RelaxedSASStateSpace)
                state = new RelaxedSASStateSpace(Declaration, state.State, state.Goals);

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

        private HashSet<Operator> ReconstructPlan(IState<Fact, Operator> state, List<Layer> graphLayers)
        {
            var selectedOperators = new HashSet<Operator>();
            var m = -1;
            foreach (var fact in state.Goals)
                m = Math.Max(m, FirstLevel(fact, graphLayers));

            if (m == -1)
                throw new RelaxedPlanningGraphException("Relaxed plan graph was not valid!");

            var G = new Dictionary<int, List<Fact>>();
            for (int t = 0; t <= m; t++)
            {
                G.Add(t, new List<Fact>());
                foreach (var fact in state.Goals)
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
