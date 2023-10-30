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

        public RelaxedPlanGenerator(PDDLDecl declaration)
        {
            Declaration = declaration;
            _generator = new RelaxedPlanningGraph();
        }

        public HashSet<Operator> GenerateReplaxedPlan(IState<Fact, Operator> state, List<Operator> operators)
        {
            Failed = false;
            if (state is not RelaxedSASStateSpace)
                state = new RelaxedSASStateSpace(Declaration, state.State, state.Goals);

            var graphLayers = _generator.GenerateRelaxedPlanningGraph(state, operators);
            if (graphLayers.Count == 0)
            {
                Failed = true;
                return new HashSet<Operator>();
            }
            var selectedActions = ReconstructPlan(state, graphLayers);

            return selectedActions;
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
                    bool found = false;
                    foreach (var act in graphLayers[t].Operators)
                    {
                        if (found)
                            break;
                        foreach(var add in act.Add)
                        {
                            if (add.Equals(fact))
                            {
                                selectedOperators.Add(act);
                                foreach(var pre in act.Pre)
                                {
                                    var newGoal = FirstLevel(pre, graphLayers);
                                    G[newGoal].Add(pre);
                                }
                                found = true;
                                break;
                            }
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
