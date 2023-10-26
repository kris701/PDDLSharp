using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.Plans;
using PDDLSharp.Toolkit.StateSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners
{
    public class RelaxedPlanningGraphs
    {
        public bool Failed { get; set; } = false;
        private HashSet<PredicateExp> _goalCache = new HashSet<PredicateExp>();
        public HashSet<ActionDecl> GenerateReplaxedPlan(IState state, HashSet<ActionDecl> groundedActions)
        {
            Failed = false;
            var graphLayers = GenerateRelaxedPlanningGraph(state, groundedActions);
            if (Failed)
                return new HashSet<ActionDecl>();
            var selectedActions = ReconstructPlan(state, graphLayers);
            if (Failed)
                return new HashSet<ActionDecl>();

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

        private HashSet<ActionDecl> ReconstructPlan(IState state, List<Layer> graphLayers)
        {
            var selectedActions = new HashSet<ActionDecl>();
            var goals = GetGoalFacts(state.Declaration.Problem);
            var m = -1;
            foreach (var fact in goals)
                m = Math.Max(m, FirstLevel(fact, graphLayers));

            if (m == -1)
            {
                Failed = true;
                return new HashSet<ActionDecl>();
            }

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

        private List<Layer> GenerateRelaxedPlanningGraph(IState state, HashSet<ActionDecl> groundedActions)
        {
            state = state.Copy();
            ActionDecl[] copyActs = new ActionDecl[groundedActions.Count];
            groundedActions.CopyTo(copyActs);
            groundedActions = copyActs.ToHashSet();
            List<Layer> layers = new List<Layer>();
            layers.Add(new Layer(new HashSet<ActionDecl>(), state.State));
            while (!state.IsInGoal())
            {
                var newLayer = new Layer();
                foreach (var act in groundedActions)
                    if (state.IsNodeTrue(act.Preconditions))
                        newLayer.Actions.Add(act);
                if (newLayer.Actions.Count == 0)
                {
                    Failed = true;
                    return new List<Layer>();
                }
                state = state.Copy();
                foreach (var act in newLayer.Actions)
                {
                    state.ExecuteNode(act.Effects);
                    groundedActions.Remove(act);
                }
                newLayer.Propositions = state.State;

                if (layers[0].Propositions == newLayer.Propositions)
                {
                    Failed = true;
                    return new List<Layer>();
                }
                layers.Add(newLayer);
            }
            return layers;
        }

        internal class Layer
        {
            public HashSet<ActionDecl> Actions { get; set; }
            public HashSet<PredicateExp> Propositions { get; set; }

            public Layer(HashSet<ActionDecl> actions, HashSet<PredicateExp> propositions)
            {
                Actions = actions;
                Propositions = propositions;
            }

            public Layer()
            {
                Actions = new HashSet<ActionDecl>();
                Propositions = new HashSet<PredicateExp>();
            }
        }
    }
}
