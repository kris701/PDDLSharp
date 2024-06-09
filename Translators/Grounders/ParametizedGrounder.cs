using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Toolkits;

namespace PDDLSharp.Translators.Grounders
{
    public class ParametizedGrounder : BaseGrounder<IParametized>
    {
        public bool RemoveStaticsFromOutput { get; set; } = false;

        private readonly HashSet<PredicateExp> _statics;
        private readonly HashSet<PredicateExp> _inits;
        private Dictionary<int, List<int[]>> _staticsViolationPatterns;
        private List<PredicateViolationCheck> _staticsPreconditions;
        public ParametizedGrounder(PDDLDecl declaration) : base(declaration)
        {
            _statics = SimpleStaticPredicateDetector.FindStaticPredicates(Declaration).ToHashSet();
            _statics.Add(new PredicateExp("=", new List<NameExp>() { new NameExp("?x"), new NameExp("?y") }));
            _inits = GenerateSimpleInits();
            _staticsViolationPatterns = new Dictionary<int, List<int[]>>();
            _staticsPreconditions = new List<PredicateViolationCheck>();
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

        public override List<IParametized> Ground(IParametized item)
        {
            List<IParametized> groundedActions = new List<IParametized>();

            if (item.Parameters.Values.Count == 0 && item.Copy() is IParametized newItem)
            {
                if (RemoveStaticsFromOutput)
                    newItem = RemoveStaticsFromNode(newItem);
                return new List<IParametized>() { newItem };
            }

            _staticsViolationPatterns = InitializeViolationPatternDict(item.Parameters.Values.Count);
            _staticsPreconditions = GenerateStaticsPreconditions(item);

            var allPermutations = GenerateParameterPermutations(item.Parameters.Values);
            if (_abort) return new List<IParametized>();
            if (RemoveStaticsFromOutput)
                item = RemoveStaticsFromNode(item);
            foreach (var permutation in allPermutations)
            {
                if (_abort) return new List<IParametized>();
                groundedActions.Add(GenerateInstance(item, permutation));
            }

            return groundedActions;
        }

        private Dictionary<int, List<int[]>> InitializeViolationPatternDict(int argCount)
        {
            var staticsViolationPatterns = new Dictionary<int, List<int[]>>();
            for (int i = 0; i < argCount; i++)
                staticsViolationPatterns.Add(i, new List<int[]>());
            return staticsViolationPatterns;
        }

        private List<PredicateViolationCheck> GenerateStaticsPreconditions(IParametized item)
        {
            switch (item)
            {
                case ActionDecl act: return GenerateStaticsViolationChecks(act.Parameters, act.Preconditions);
                case AxiomDecl axi: return GenerateStaticsViolationChecks(axi.Parameters, axi.Context);
                case DurativeActionDecl dAct: return GenerateStaticsViolationChecks(dAct.Parameters, dAct.Condition);
                case ForAllExp forAll: return new List<PredicateViolationCheck>();
                case ExistsExp exists: return new List<PredicateViolationCheck>();
                default:
                    throw new Exception("Invalid object given to grounder!");
            }
        }

        private List<PredicateViolationCheck> GenerateStaticsViolationChecks(ParameterExp param, INode exp)
        {
            var staticsPreconditions = new List<PredicateViolationCheck>();
            var argumentIndexes = new Dictionary<string, int>();
            int index = 0;
            foreach (var arg in param.Values)
                argumentIndexes.Add(arg.Name, index++);
            var allPredicates = exp.FindTypes<PredicateExp>();
            foreach (var stat in _statics)
            {
                var allRefs = allPredicates.Where(x => x.Name == stat.Name);
                foreach (var refPred in allRefs)
                {
                    bool valid = true;
                    var argIndexes = new int[stat.Arguments.Count];
                    var constantIndexes = new int[stat.Arguments.Count];
                    for (int i = 0; i < refPred.Arguments.Count; i++)
                    {
                        if (argumentIndexes.ContainsKey(refPred.Arguments[i].Name))
                        {
                            argIndexes[i] = argumentIndexes[refPred.Arguments[i].Name];
                            constantIndexes[i] = int.MaxValue;
                        }
                        else
                        {
                            if (refPred.Arguments[i].Name.Contains("?"))
                            {
                                valid = false;
                                break;
                            }
                            argIndexes[i] = int.MaxValue;
                            constantIndexes[i] = GetIndexFromObject(refPred.Arguments[i].Name);
                        }
                    }
                    if (valid)
                        staticsPreconditions.Add(new PredicateViolationCheck(stat, argIndexes, constantIndexes, refPred.Parent is not NotExp));
                }
            }

            return staticsPreconditions;
        }

        private IParametized RemoveStaticsFromNode(IParametized item)
        {
            var copy = item.Copy();
            foreach (var statics in _statics)
            {
                var allRefs = copy.FindTypes<PredicateExp>();
                var allStaticsRef = allRefs.Where(x => x.Name == statics.Name);
                foreach (var reference in allStaticsRef)
                {
                    if (reference.Parent is IListable list)
                        list.Remove(reference);
                    else if (reference.Parent is NotExp not && not.Parent is IListable list2)
                        list2.Remove(reference);
                    else if (statics.Name == "=" && reference.Parent is NotExp not2 && not2.Parent is IListable list3)
                        list3.Remove(not2);
                }
            }
            if (copy is IParametized param)
                return param;
            throw new ArgumentException("Expected copy to be a `IParametizzed`!");
        }

        internal override bool IsPermutationLegal(int[] permutation, int index)
        {
            if (_abort) return false;
            if (_statics.Count > 0)
            {
                if (!IsPermutationLegal(permutation, index + 1, _staticsViolationPatterns))
                    return false;

                bool allGood = true;
                foreach (var staticsPrecon in _staticsPreconditions)
                {
                    bool generatePattern = false;

                    if (staticsPrecon.Predicate.Name == "=")
                    {
                        var arg1 = -1;
                        if (staticsPrecon.ArgIndexes[0] != int.MaxValue)
                            arg1 = permutation[staticsPrecon.ArgIndexes[0]];
                        if (staticsPrecon.ConstantsIndexes[0] != int.MaxValue)
                            arg1 = staticsPrecon.ConstantsIndexes[0];

                        var arg2 = -1;
                        if (staticsPrecon.ArgIndexes[1] != int.MaxValue)
                            arg2 = permutation[staticsPrecon.ArgIndexes[1]];
                        if (staticsPrecon.ConstantsIndexes[1] != int.MaxValue)
                            arg2 = staticsPrecon.ConstantsIndexes[1];

                        if (staticsPrecon.IsTrue)
                        {
                            if (arg1 != arg2)
                                generatePattern = true;
                        }
                        else
                        {
                            if (arg1 == arg2)
                                generatePattern = true;
                        }
                    }
                    else
                    {
                        if (staticsPrecon.IsTrue)
                            generatePattern = !_inits.Contains(GeneratePredicateFromIndexes(permutation, staticsPrecon));
                        else
                            generatePattern = _inits.Contains(GeneratePredicateFromIndexes(permutation, staticsPrecon));
                    }

                    if (generatePattern)
                    {
                        int minIndex = staticsPrecon.ArgIndexes.Min();
                        minIndex = Math.Min(minIndex, staticsPrecon.ConstantsIndexes.Min());
                        var maxIndex = -1;
                        for(int i = 0; i < staticsPrecon.ArgIndexes.Length; i++)
                        {
                            if (staticsPrecon.ArgIndexes[i] > maxIndex && staticsPrecon.ConstantsIndexes[i] == int.MaxValue)
                                maxIndex = staticsPrecon.ArgIndexes[i];
                        }

                        if (maxIndex <= index)
                        {
                            _staticsViolationPatterns[minIndex].Add(GeneratePattern(permutation, index, staticsPrecon));
                            allGood = false;
                        }
                    }
                }
                if (!allGood)
                    return false;
            }
            return true;
        }

        private PredicateExp GeneratePredicateFromIndexes(int[] permutation, PredicateViolationCheck staticsPrecon)
        {
            var newArgs = new List<NameExp>();
            for (int i = 0; i < staticsPrecon.ArgIndexes.Length; i++)
            {
                if (staticsPrecon.ArgIndexes[i] == int.MaxValue)
                    newArgs.Add(new NameExp(GetObjectFromIndex(staticsPrecon.ConstantsIndexes[i])));
                else
                    newArgs.Add(new NameExp(GetObjectFromIndex(permutation[staticsPrecon.ArgIndexes[i]])));
            }
            return new PredicateExp(staticsPrecon.Predicate.Name, newArgs);
        }

        private int[] GeneratePattern(int[] permutation, int index, PredicateViolationCheck staticsPrecon)
        {
            var newPattern = new int[index + 1];
            var covered = new bool[staticsPrecon.ArgIndexes.Length];
            for (int i = 0; i < newPattern.Length; i++)
            {
                bool any = false;
                for (int j = 0; j < staticsPrecon.ArgIndexes.Length; j++)
                {
                    if (i == staticsPrecon.ArgIndexes[j] && !covered[j])
                    {
                        newPattern[i] = permutation[staticsPrecon.ArgIndexes[j]];
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

        private bool IsPermutationLegal(int[] permutation, int index, Dictionary<int, List<int[]>> violationPatterns)
        {
            // Check forward in patterns, since the permutations is generated backwards
            int violated;
            int expected;
            int length = permutation.Length;
            for (int i = 0; i < length; i++)
            {
                if (!violationPatterns.ContainsKey(i))
                    continue;
                foreach (var pattern in violationPatterns[i])
                {
                    if (pattern.Length > index)
                        continue;
                    violated = 0;
                    expected = 0;
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

        private IParametized GenerateInstance(IParametized item, int[] permutation)
        {
            var copy = item.Copy();
            for (int i = 0; i < item.Parameters.Values.Count; i++)
            {
                var allRefs = copy.FindNames(item.Parameters.Values[i].Name);
                foreach (var refItem in allRefs)
                    refItem.Name = GetObjectFromIndex(permutation[i]);
            }
            copy.RemoveTypes();
            copy.RemoveContext();
            if (copy is IParametized param)
                return param;

            throw new Exception("Could not create instance of object!");
        }
    }
}
