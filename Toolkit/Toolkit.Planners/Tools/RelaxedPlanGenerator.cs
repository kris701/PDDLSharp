using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Toolkit.StateSpace;

namespace PDDLSharp.Toolkit.Planners.Tools
{
    public class RelaxedPlanGenerator
    {
        public PDDLDecl Declaration { get; set; }
        private HashSet<PredicateExp> _goalCache;

        public RelaxedPlanGenerator(PDDLDecl declaration)
        {
            Declaration = declaration;
            _goalCache = new HashSet<PredicateExp>();
            GetGoalFacts(declaration.Problem);
        }

        public HashSet<ActionDecl> GenerateReplaxedPlan(IState state, List<ActionDecl> groundedActions)
        {
            if (state is not RelaxedPDDLStateSpace)
                state = new RelaxedPDDLStateSpace(Declaration, state.State, state.Grounder);
            
            var graphLayers = RelaxedPlanningGraph.GenerateRelaxedPlanningGraph(state, groundedActions);
            var selectedActions = ReconstructPlan(state, graphLayers);

            return selectedActions;
        }

        private HashSet<ActionDecl> ReconstructPlan(IState state, List<Layer> graphLayers)
        {
            var selectedActions = new HashSet<ActionDecl>();
            var goals = GetGoalFacts(state.Declaration.Problem);
            var m = -1;
            foreach (var fact in goals)
                m = Math.Max(m, FirstLevel(fact, graphLayers));

            if (m == -1)
                throw new Exception("Relaxed plan graph was not valid!");

            var G = new Dictionary<int, List<PredicateExp>>();
            for (int t = 0; t <= m; t++)
            {
                G.Add(t, new List<PredicateExp>());
                foreach (var fact in goals)
                    if (FirstLevel(fact, graphLayers) == t)
                        G[t].Add(fact);
            }

            for (int t = m; t >= 0; t--)
            {
                foreach (var fact in G[t])
                {
                    foreach (var act in graphLayers[t].Actions)
                    {
                        var allEff = act.Effects.FindTypes<PredicateExp>();
                        if (allEff.Any(x => x.Parent is not NotExp && SimplifyPredicate(x).Equals(fact)))
                        {
                            selectedActions.Add(act);
                            var allPrecons = act.Preconditions.FindTypes<PredicateExp>();
                            foreach (var precon in allPrecons)
                            {
                                if (precon is PredicateExp pred)
                                {
                                    var newGoal = FirstLevel(pred, graphLayers);
                                    G[newGoal].Add(pred);
                                }
                            }
                        }
                    }
                }
            }

            return selectedActions;
        }

        private HashSet<PredicateExp> GetGoalFacts(ProblemDecl problem)
        {
            if (_goalCache.Count != 0)
                return _goalCache;
            var extracted = new HashSet<PredicateExp>();
            if (problem.Goal != null)
            {
                var allPreds = problem.Goal.GoalExp.FindTypes<PredicateExp>();
                foreach (var pred in allPreds)
                    if (pred.Parent is not NotExp)
                        extracted.Add(pred);
            }
            var simplified = new HashSet<PredicateExp>();
            foreach (var fact in extracted)
                simplified.Add(SimplifyPredicate(fact));

            _goalCache = simplified;
            return _goalCache;
        }

        private PredicateExp SimplifyPredicate(PredicateExp pred)
        {
            var newPred = new PredicateExp(pred.Name);
            foreach (var arg in pred.Arguments)
                newPred.Arguments.Add(new NameExp(arg.Name));
            return newPred;
        }

        private int FirstLevel(PredicateExp fact, List<Layer> layers)
        {
            for (int i = 0; i < layers.Count; i++)
                if (layers[i].Propositions.Contains(fact))
                    return i;
            return -1;
        }
    }
}
