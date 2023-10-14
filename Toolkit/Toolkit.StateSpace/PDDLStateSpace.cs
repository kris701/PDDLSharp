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
            if (node is PredicateExp predicate)
            {
                if (isNegative)
                    _tempDel.Add(predicate);
                else
                    _tempAdd.Add(predicate);
                return;
            }
            else if (node is NotExp not)
            {
                ExecuteNode(not.Child, !isNegative);
                return;
            }
            else if (node is AndExp and)
            {
                foreach (var child in and.Children)
                    ExecuteNode(child, isNegative);
                return;
            }
            else if (node is WhenExp when)
            {
                if (IsNodeTrue(when.Condition))
                    ExecuteNode(when.Effect, false);
                return;
            }
            else if (node is ForAllExp all)
            {
                CheckPermutationsStepwise(
                    all.Expression,
                    all.Parameters,
                    (x) => {
                        ExecuteNode(x, isNegative);
                        return null;
                    });
                return;
            }
            else if (node is NumericExp num)
                return;

            throw new Exception($"Unknown node type to evaluate! '{node.GetType()}'");
        }

        public bool IsNodeTrue(INode node)
        {
            if (node is DerivedPredicateExp derivedPredicate)
            {
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
            }
            else if (node is PredicateExp predicate)
            {
                // Handle Equality predicate
                if (predicate.Name == "=" && predicate.Arguments.Count == 2)
                    return predicate.Arguments[0].Name == predicate.Arguments[1].Name;

                return Contains(predicate);
            }
            else if (node is NotExp not)
            {
                return !IsNodeTrue(not.Child);
            }
            else if (node is OrExp or)
            {
                foreach (var subNode in or)
                    if (IsNodeTrue(subNode))
                        return true;
                return false;
            }
            else if (node is AndExp and)
            {
                foreach (var subNode in and)
                    if (!IsNodeTrue(subNode))
                        return false;
                return true;
            }
            else if (node is ExistsExp exist)
            {
                return CheckPermutationsStepwise(
                    exist.Expression,
                    exist.Parameters,
                    (x) => {
                        if (IsNodeTrue(x))
                            return true;
                        return null;
                    },
                    false);
            }
            else if (node is ImplyExp imply)
            {
                if (IsNodeTrue(imply.Antecedent) && IsNodeTrue(imply.Consequent))
                    return true;
                if (!IsNodeTrue(imply.Antecedent))
                    return true;
                return false;
            }
            else if (node is ForAllExp all)
            {
                return CheckPermutationsStepwise(
                    all.Expression, 
                    all.Parameters, 
                    (x) => {
                        if (!IsNodeTrue(x))
                            return false;
                        return null;
                    });
            }
            else if (node is WhenExp when)
            {
                if (IsNodeTrue(when.Condition))
                    return IsNodeTrue(when.Effect);
                return false;
            }

            throw new Exception($"Unknown node type to evaluate! '{node.GetType()}'");
        }

        private bool CheckPermutationsStepwise(INode node, ParameterExp parameters, Func<INode, bool?> stopFunc, bool defaultReturn = true)
        {
            var allPermuations = GenerateParameterPermutations(parameters);
            for (int i = 0; i < allPermuations.Count; i++) {
                var res = stopFunc(GenerateNewParametized(node, parameters, allPermuations[i]));
                if (res != null)
                    return (bool)res;
            }
            return defaultReturn;
        }

        private INode GenerateNewParametized(INode node, ParameterExp replace, ParameterExp with)
        {
            var checkNode = node.Copy();
            for(int i = 0; i < replace.Values.Count; i++)
            {
                var allRefs = checkNode.FindNames(replace.Values[i].Name);
                foreach (var name in allRefs)
                    name.Name = with.Values[i].Name;
            }

            return checkNode;
        }

        private List<ParameterExp> GenerateParameterPermutations(ParameterExp parameters, int index = 0)
        {
            List<ParameterExp> returnList = new List<ParameterExp>();

            List<NameExp> allOfType = GetObjsForType(parameters.Values[index].Type.Name);
            foreach (var ofType in allOfType)
            {
                var newParam = parameters.Copy();
                newParam.Values[index] = ofType;
                if (index >= parameters.Values.Count - 1)
                    returnList.Add(newParam);
                else
                    returnList.AddRange(GenerateParameterPermutations(newParam, index + 1));
            }

            return returnList;
        }

        private Dictionary<string, List<NameExp>> _objCache = new Dictionary<string, List<NameExp>>();
        private List<NameExp> GetObjsForType(string typeName)
        {
            if (_objCache.ContainsKey(typeName))
                return _objCache[typeName];
            _objCache.Add(typeName, new List<NameExp>());
            if (Declaration.Problem.Objects != null)
                _objCache[typeName].AddRange(Declaration.Problem.Objects.Objs.Where(x => x.Type.IsTypeOf(typeName)));
            if (Declaration.Domain.Constants != null)
                _objCache[typeName].AddRange(Declaration.Domain.Constants.Constants.Where(x => x.Type.IsTypeOf(typeName)));
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
