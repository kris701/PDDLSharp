using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using System.Xml.Linq;

namespace PDDLSharp.Toolkit.StateSpace
{
    public class PDDLStateSpace
    {
        public PDDLDecl Declaration { get; internal set; }
        private HashSet<PredicateExp> _state;
        private List<PredicateExp> _tempAdd = new List<PredicateExp>();
        private List<PredicateExp> _tempDel = new List<PredicateExp>();

        public PDDLStateSpace(PDDLDecl declaration)
        {
            Declaration = declaration;
            _state = new HashSet<PredicateExp>();
            if (declaration.Problem.Init != null)
                foreach (var item in declaration.Problem.Init.Predicates)
                    if (item is PredicateExp predicate)
                        Add(SimplifyPredicate(predicate));
        }

        public int Count => _state.Count;

        private PredicateExp SimplifyPredicate(PredicateExp pred)
        {
            var newPred = new PredicateExp(pred.Name);
            foreach (var arg in pred.Arguments)
                newPred.Arguments.Add(new NameExp(arg.Name));
            return newPred;
        }

        private PredicateExp SimplifyPredicate(string predicate, params string[] arguments)
        {
            var newPred = new PredicateExp(predicate);
            foreach (var arg in arguments)
                newPred.Arguments.Add(new NameExp(arg));
            return newPred;
        }

        public void Add(PredicateExp pred) => _state.Add(SimplifyPredicate(pred));
        public void Add(string pred, params string[] arguments) => Add(SimplifyPredicate(pred, arguments));
        public void Del(PredicateExp pred) => _state.Remove(SimplifyPredicate(pred));
        public void Del(string pred, params string[] arguments) => Del(SimplifyPredicate(pred, arguments));
        public bool Contains(PredicateExp pred) => _state.Contains(SimplifyPredicate(pred));
        public bool Contains(string pred, params string[] arguments) => Contains(SimplifyPredicate(pred, arguments));

        public void ExecuteNode(INode node)
        {
            _tempAdd.Clear();
            _tempDel.Clear();
            ExecuteNode(node, false);
            foreach (var item in _tempDel)
                Del(item);
            foreach (var item in _tempAdd)
                Add(item);
        }
        private void ExecuteNode(INode node, bool isNegative)
        {
            switch (node)
            {
                case PredicateExp predicate:
                    if (isNegative)
                        _tempDel.Add(predicate);
                    else
                        _tempAdd.Add(predicate);
                    return;
                case NotExp not:
                    ExecuteNode(not.Child, !isNegative);
                    return;
                case AndExp and:
                    foreach (var child in and.Children)
                        ExecuteNode(child, isNegative);
                    return;
                case ForAllExp all:
                    CheckPermutationsStepwise(
                    all.Expression,
                    all.Parameters,
                    (x) => {
                        ExecuteNode(x, isNegative);
                        return null;
                    });
                    return;
                case WhenExp whe:
                    if (IsNodeTrue(whe.Condition))
                        ExecuteNode(whe.Effect, false);
                    return;
                case NumericExp num:
                    return;
            }

            throw new Exception($"Unknown node type to evaluate! '{node.GetType()}'");
        }

        public bool IsNodeTrue(INode node)
        {
            switch (node)
            {
                case DerivedPredicateExp derivedPredicate:
                    foreach (var derivedDecl in derivedPredicate.GetDecls())
                    {
                        var newTestNode = derivedDecl.Expression.Copy();
                        for (int i = 0; i < derivedDecl.Predicate.Arguments.Count; i++)
                        {
                            var all = newTestNode.FindNames(derivedDecl.Predicate.Arguments[i].Name);
                            foreach (var name in all)
                                name.Name = derivedPredicate.Arguments[i].Name;
                        }
                        if (IsNodeTrue(newTestNode))
                            return true;
                    }
                    return false;
                case PredicateExp predicate:
                    // Handle Equality predicate
                    if (predicate.Name == "=" && predicate.Arguments.Count == 2)
                        return predicate.Arguments[0].Name == predicate.Arguments[1].Name;

                    return Contains(predicate);
                case NotExp not:
                    return !IsNodeTrue(not.Child);
                case OrExp or:
                    foreach (var subNode in or)
                        if (IsNodeTrue(subNode))
                            return true;
                    return false;
                case AndExp and:
                    foreach (var subNode in and)
                        if (!IsNodeTrue(subNode))
                            return false;
                    return true;
                case ExistsExp exist:
                    return CheckPermutationsStepwise(
                        exist.Expression,
                        exist.Parameters,
                        (x) => {
                            if (IsNodeTrue(x))
                                return true;
                            return null;
                        },
                        false);
                case ImplyExp imply:
                    if (IsNodeTrue(imply.Antecedent) && IsNodeTrue(imply.Consequent))
                        return true;
                    if (!IsNodeTrue(imply.Antecedent))
                        return true;
                    return false;
                case ForAllExp all:
                    return CheckPermutationsStepwise(
                        all.Expression,
                        all.Parameters,
                        (x) => {
                            if (!IsNodeTrue(x))
                                return false;
                            return null;
                        });
                case WhenExp whe:
                    if (IsNodeTrue(whe.Condition))
                        return IsNodeTrue(whe.Effect);
                    return false;
            }

            throw new Exception($"Unknown node type to evaluate! '{node.GetType()}'");
        }

        private bool CheckPermutationsStepwise(INode node, ParameterExp parameters, Func<INode, bool?> stopFunc, bool defaultReturn = true)
        {
            var allPermuations = GenerateParameterPermutations(parameters.Values, new List<string>(parameters.Values.Count));
            for (int i = 0; i < allPermuations.Count; i++) {
                var res = stopFunc(GenerateNewParametized(node, parameters, allPermuations[i]));
                if (res != null)
                    return (bool)res;
            }
            return defaultReturn;
        }

        private INode GenerateNewParametized(INode node, ParameterExp replace, List<string> with)
        {
            var checkNode = node.Copy();
            for(int i = 0; i < replace.Values.Count; i++)
            {
                var allRefs = checkNode.FindNames(replace.Values[i].Name);
                foreach (var name in allRefs)
                    name.Name = with[i];
            }

            return checkNode;
        }

        private List<List<string>> GenerateParameterPermutations(List<NameExp> parameters, List<string> carried, int index = 0)
        {
            List<List<string>> returnList = new List<List<string>>();

            List<string> allOfType = GetObjsForType(parameters[index].Type.Name);
            foreach (var ofType in allOfType)
            {
                var newParam = new List<string>(carried);
                newParam.Add(ofType);
                if (index >= parameters.Count - 1)
                    returnList.Add(newParam);
                else
                    returnList.AddRange(GenerateParameterPermutations(parameters, newParam, index + 1));
            }

            return returnList;
        }

        private Dictionary<string, List<string>> _objCache = new Dictionary<string, List<string>>();
        private List<string> GetObjsForType(string typeName)
        {
            if (_objCache.ContainsKey(typeName))
                return _objCache[typeName];

            var addItems = new List<NameExp>();
            if (Declaration.Problem.Objects != null)
                addItems.AddRange(Declaration.Problem.Objects.Objs.Where(x => x.Type.IsTypeOf(typeName)));
            if (Declaration.Domain.Constants != null)
                addItems.AddRange(Declaration.Domain.Constants.Constants.Where(x => x.Type.IsTypeOf(typeName)));

            _objCache.Add(typeName, new List<string>());
            foreach (var item in addItems)
                _objCache[typeName].Add(item.Name);
            return _objCache[typeName];
        }

        public bool IsInGoal()
        {
            if (Declaration.Problem.Goal == null)
                throw new ArgumentNullException("No problem goal was declared!");
            return IsNodeTrue(Declaration.Problem.Goal.GoalExp);
        }
    }
}
