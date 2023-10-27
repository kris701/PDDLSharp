using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Toolkit.StaticPredicateDetectors;
using System;
using System.Linq;

namespace PDDLSharp.Toolkit.Grounders
{
    public class ActionGrounder : BaseGrounder<ActionDecl>
    {
        private HashSet<PredicateExp> _statics;
        private HashSet<PredicateExp> _inits;
        public ActionGrounder(PDDLDecl declaration) : base(declaration)
        {
            var staticPredicateDetector = new SimpleStaticPredicateDetector();
            _statics = staticPredicateDetector.FindStaticPredicates(Declaration).ToHashSet();
            _inits = GenerateSimpleInits();
        }

        public override List<ActionDecl> Ground(ActionDecl item)
        {
            List<ActionDecl> groundedActions = new List<ActionDecl>();

            if (item.Parameters.Values.Count == 0 && item.Copy() is ActionDecl newItem)
                return new List<ActionDecl>() { newItem };

            var staticsViolationPatterns = new Dictionary<int, List<int[]>>();
            for(int i = 0; i < item.Parameters.Values.Count; i++)
                staticsViolationPatterns.Add(i, new List<int[]>());

            var staticsPreconditions = GenerateStaticsViolationChecks(item, _statics);

            var allPermutations = GenerateParameterPermutations(item.Parameters.Values);
            foreach (var permutation in allPermutations)
            {
                if (_statics.Count > 0)
                {
                    if (!IsPermutationLegal(permutation, staticsViolationPatterns))
                        continue;

                    bool allGood = true;
                    foreach (var staticsPrecon in staticsPreconditions)
                    {
                        if (!_inits.Contains(GeneratePredicateFromIndexes(permutation, staticsPrecon)))
                        {
                            int maxIndex = staticsPrecon.Indexes.Max();
                            staticsViolationPatterns[maxIndex].Add(GeneratePattern(permutation, staticsPrecon));
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

        private PredicateExp GeneratePredicateFromIndexes(int[] permutation, PredicateViolationCheck staticsPrecon)
        {
            var newArgs = new List<NameExp>();
            for (int i = 0; i < staticsPrecon.Indexes.Length; i++)
                newArgs.Add(new NameExp(GetObjectFromIndex(permutation[staticsPrecon.Indexes[i]])));
            return new PredicateExp(staticsPrecon.Predicate.Name, newArgs);
        }

        private int[] GeneratePattern(int[] permutation, PredicateViolationCheck staticsPrecon)
        {
            var newPattern = new int[permutation.Length];
            var covered = new bool[staticsPrecon.Indexes.Length];
            for (int i = 0; i < newPattern.Length; i++)
            {
                bool any = false;
                for (int j = 0; j < staticsPrecon.Indexes.Length; j++)
                {
                    if (i == staticsPrecon.Indexes[j] && !covered[j])
                    {
                        newPattern[i] = permutation[staticsPrecon.Indexes[j]];
                        covered[j] = true;
                        any = true;
                        break;
                    }
                }
                if (!any)
                    newPattern[i] = -1;
            }
            return newPattern;
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

        private List<PredicateViolationCheck> GenerateStaticsViolationChecks(ActionDecl action, HashSet<PredicateExp> statics)
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

        private bool IsPermutationLegal(int[] permutation, Dictionary<int, List<int[]>> violationPatterns)
        {
            // Check forward in patterns, since the permutations is generated backwards
            int violated;
            int expected;
            int length = permutation.Length;
            for (int i = 0; i < length; i++)
            {
                foreach(var pattern in violationPatterns[i])
                {
                    violated = 0;
                    expected = 0;
                    for (int j = 0; j < length; j++)
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
            copy.RemoveContext();
            return copy;
        }
    }
}
