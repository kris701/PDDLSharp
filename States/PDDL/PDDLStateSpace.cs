using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.Plans;

namespace PDDLSharp.States.PDDL
{
    public class PDDLStateSpace : IPDDLState
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
                        Add(predicate);
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
                var permutations = GenerateParameterPermutations(all.Expression, all.Parameters.Values, 0);
                foreach (var permutation in permutations)
                    ExecuteNode(permutation, isNegative);
                return;
            }
            else if (node is NumericExp num)
                return;

            throw new Exception($"Unknown node type to evaluate! '{node.GetType()}'");
        }

        private List<INode> GenerateParameterPermutations(INode node, List<NameExp> values, int index)
        {
            List<INode> returnList = new List<INode>();

            if (index >= values.Count)
            {
                returnList.Add(node);
                return returnList;
            }

            List<NameExp> allOfType = new List<NameExp>();
            if (Declaration.Problem.Objects != null)
                allOfType.AddRange(Declaration.Problem.Objects.Objs.Where(x => x.Type.IsTypeOf(values[index].Type.Name)));
            if (Declaration.Domain.Constants != null)
                allOfType.AddRange(Declaration.Domain.Constants.Constants.Where(x => x.Type.IsTypeOf(values[index].Type.Name)));
            foreach (var ofType in allOfType)
            {
                var newNode = node.Copy(null);
                var allNames = newNode.FindNames(values[index].Name);
                foreach (var name in allNames)
                    name.Name = ofType.Name;
                returnList.AddRange(GenerateParameterPermutations(newNode, values, index + 1));
            }

            return returnList;
        }

        public bool IsNodeTrue(INode node)
        {
            if (node is PredicateExp predicate)
            {
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
                var permutations = GenerateParameterPermutations(exist.Expression, exist.Parameters.Values, 0);
                foreach (var permutation in permutations)
                    if (IsNodeTrue(permutation))
                        return true;
                return false;
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
                var permutations = GenerateParameterPermutations(all.Expression, all.Parameters.Values, 0);
                foreach (var permutation in permutations)
                    if (!IsNodeTrue(permutation))
                        return false;
                return true;
            }

            throw new Exception($"Unknown node type to evaluate! '{node.GetType()}'");
        }

        public bool IsInGoal()
        {
            if (Declaration.Problem.Goal == null)
                throw new ArgumentNullException("No problem goal was declared!");
            return IsNodeTrue(Declaration.Problem.Goal.GoalExp);
        }
    }
}
