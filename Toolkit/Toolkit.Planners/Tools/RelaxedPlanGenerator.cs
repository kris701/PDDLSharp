using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Toolkit.Planners.Exceptions;
using PDDLSharp.Toolkit.StateSpace;

namespace PDDLSharp.Toolkit.Planners.Tools
{
    public class RelaxedPlanGenerator
    {
        public bool Failed { get; internal set; } = false;
        public PDDLDecl Declaration { get; set; }
        private HashSet<PredicateExp> _goalCache;
        private RelaxedPlanningGraph _generator;

        public RelaxedPlanGenerator(PDDLDecl declaration)
        {
            Declaration = declaration;
            _goalCache = new HashSet<PredicateExp>();
            _generator = new RelaxedPlanningGraph();
            GetGoalFacts(declaration.Problem);
        }

        public HashSet<ActionDecl> GenerateReplaxedPlan(IState state, List<ActionDecl> groundedActions)
        {
            Failed = false;
            if (state is not RelaxedPDDLStateSpace)
                state = new RelaxedPDDLStateSpace(Declaration, state.State, state.Grounder);
            
            var graphLayers = _generator.GenerateRelaxedPlanningGraph(state, groundedActions);
            if (graphLayers.Count == 0)
            {
                Failed = true;
                return new HashSet<ActionDecl>();
            }
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
                throw new RelaxedPlanningGraphException("Relaxed plan graph was not valid!");

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
                    bool found = false;
                    foreach (var act in graphLayers[t].Actions) 
                    {
                        if (found)
                            break;
                        if (act.Effects is AndExp effAnd)
                        {
                            foreach (var child in effAnd.Children)
                            {
                                if (child is PredicateExp pred && SimplifyPredicate(pred).Equals(fact))
                                {
                                    selectedActions.Add(act);
                                    if (act.Preconditions is AndExp preAnd)
                                    {
                                        foreach (var child2 in preAnd.Children)
                                        {
                                            if (child2 is PredicateExp pred2)
                                            {
                                                var newGoal = FirstLevel(pred2, graphLayers);
                                                G[newGoal].Add(pred2);
                                            }
                                        }
                                    }
                                    else
                                        throw new RelaxedPlanningGraphException("Expected action preconditions to be an and expression!");
                                    found = true;
                                    break;
                                }
                            }
                        }
                        else
                            throw new RelaxedPlanningGraphException("Expected action effects to be an and expression!");
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
