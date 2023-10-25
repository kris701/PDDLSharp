using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Toolkit.StaticPredicateDetectors;
using System;

namespace PDDLSharp.Toolkit.Grounders
{
    public class ActionGrounder : BaseGrounder<ActionDecl>
    {
        IStaticPredicateDetectors _staticPredicateDetector;
        public ActionGrounder(PDDLDecl declaration) : base(declaration)
        {
            _staticPredicateDetector = new SimpleStaticPredicateDetector();
        }

        public override List<ActionDecl> Ground(ActionDecl item)
        {
            List<ActionDecl> groundedActions = new List<ActionDecl>();

            if (item.Parameters.Values.Count == 0 && item.Copy() is ActionDecl newItem)
                return new List<ActionDecl>() { newItem };

            var statics = _staticPredicateDetector.FindStaticPredicates(Declaration);
            var violationPatterns = new HashSet<int[]>();
            var simpleInits = GenerateSimpleInits();
            var staticsPreconditions = GenerateStaticsViolationChecks(item, statics);

            var allPermutations = GenerateParameterPermutations(item.Parameters.Values);
            foreach (var permutation in allPermutations)
            {
                if (statics.Count > 0)
                {
                    if (!IsPermutationLegal(permutation, violationPatterns))
                        continue;

                    bool allGood = true;
                    foreach (var staticsPrecon in staticsPreconditions)
                    {
                        var newArgs = new List<NameExp>();
                        for (int i = 0; i < staticsPrecon.Indexes.Length; i++)
                            newArgs.Add(new NameExp(GetObjectFromIndex(permutation[staticsPrecon.Indexes[i]])));
                        if (!simpleInits.Contains(new PredicateExp(staticsPrecon.Predicate.Name, newArgs)))
                        {
                            var newPattern = new int[permutation.Length];
                            int index = 0;
                            for (int i = 0; i < newPattern.Length; i++)
                            {
                                if (index < staticsPrecon.Indexes.Length && i == staticsPrecon.Indexes[index])
                                    newPattern[i] = permutation[staticsPrecon.Indexes[index++]];
                                else
                                    newPattern[i] = -1;
                            }
                            violationPatterns.Add(newPattern);
                            allGood = false;
                        }
                    }
                    if (!allGood)
                        continue;
                }

                var copy = GenerateActionInstance(item, permutation);
                groundedActions.Add(copy);
            }

            return groundedActions;
        }

        private HashSet<PredicateExp> GenerateSimpleInits()
        {
            var simpleInits = new HashSet<PredicateExp>();
            if (Declaration.Problem.Init != null)
            {
                foreach (var init in Declaration.Problem.Init.Predicates)
                {
                    if (init is PredicateExp pred)
                    {
                        var newArgs = new List<NameExp>();
                        foreach (var arg in pred.Arguments)
                            newArgs.Add(new NameExp(arg.Name));
                        simpleInits.Add(new PredicateExp(pred.Name, newArgs));
                    }
                }
            }
            return simpleInits;
        }

        private List<PredicateViolationCheck> GenerateStaticsViolationChecks(ActionDecl action, List<PredicateExp> statics)
        {
            var staticsPreconditions = new List<PredicateViolationCheck>();
            var argumentIndexes = new Dictionary<string, int>();
            int index = 0;
            foreach (var arg in action.Parameters.Values)
                argumentIndexes.Add(arg.Name, index++);
            var allPredicates = action.Preconditions.FindTypes<PredicateExp>();
            foreach (var stat in statics)
            {
                var allRefs = allPredicates.Where(x => x.Name == stat.Name);
                foreach (var refPred in allRefs)
                {
                    var indexes = new int[refPred.Arguments.Count];
                    for (int i = 0; i < refPred.Arguments.Count; i++)
                        indexes[i] = argumentIndexes[refPred.Arguments[i].Name];
                    staticsPreconditions.Add(new PredicateViolationCheck(stat, indexes));
                }
            }
            return staticsPreconditions;
        }

        private bool IsPermutationLegal(int[] permutation, HashSet<int[]> violationPatterns)
        {
            for (int i = 0; i < permutation.Length; i++)
            {
                foreach(var pattern in violationPatterns)
                {
                    int violated = 0;
                    int expected = 0;
                    for (int j = 0; j < pattern.Length; j++)
                    {
                        if (pattern[j] != -1)
                        {
                            expected++;
                            if (pattern[j] == permutation[j])
                                violated++;
                        }
                    }
                    if (violated == expected)
                        return false;
                }
            }
            return true;
        }

        private ActionDecl GenerateActionInstance(ActionDecl action, int[] permutation)
        {
            var copy = action.Copy();
            for (int i = 0; i < action.Parameters.Values.Count; i++)
            {
                var allRefs = copy.FindNames(action.Parameters.Values[i].Name);
                foreach (var refItem in allRefs)
                    refItem.Name = GetObjectFromIndex(permutation[i]);
            }
            return copy;
        }
    }
}
