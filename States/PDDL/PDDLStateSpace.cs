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
        private HashSet<PredicateExp> _tempAdd = new HashSet<PredicateExp>();
        private HashSet<PredicateExp> _tempDel = new HashSet<PredicateExp>();

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
            foreach (var item in _tempAdd)
                Add(item);
            foreach (var item in _tempDel)
                Del(item);
        }
        private void ExecuteNode(INode node, bool isNegative)
        {
            if (node is PredicateExp predicate)
            {
                if (isNegative)
                    _tempDel.Add(predicate);
                else
                    _tempAdd.Add(predicate);
            }
            else if (node is NotExp not)
            {
                ExecuteNode(not.Child, !isNegative);
            }
            else if (node is WhenExp when)
            {
                if (IsNodeTrue(when.Condition, false))
                    ExecuteNode(when.Effect, false);
            }
            else if (node is ForAllExp all)
            {
                var permutations = GenerateParameterPermutations(all.Expression, all.Parameters.Values, 0);
                foreach (var permutation in permutations)
                    ExecuteNode(permutation, isNegative);
            }
            else if (node is IWalkable walk)
            {
                foreach (var subNode in walk)
                    ExecuteNode(subNode, isNegative);
            }
        }

        private List<IExp> GenerateParameterPermutations(IExp node, List<NameExp> values, int index)
        {
            List<IExp> returnList = new List<IExp>();

            if (index >= values.Count)
            {
                returnList.Add(node);
                return returnList;
            }

            if (Declaration.Problem.Objects != null)
            {
                var allOfType = Declaration.Problem.Objects.Objs.Where(x => x.Type.IsTypeOf(values[index].Type.Name));
                foreach (var ofType in allOfType)
                {

                    returnList.AddRange(GenerateParameterPermutations(node, values, index + 1));
                }
            }

            return returnList;
        }

        public bool IsNodeTrue(INode node) => IsNodeTrue(node, false);
        private bool IsNodeTrue(INode node, bool isNegative)
        {
            if (node is PredicateExp predicate)
            {
                if (isNegative)
                    return !Contains(predicate);
                else
                    return Contains(predicate);
            }
            else if (node is NotExp not)
            {
                return IsNodeTrue(not.Child, !isNegative);
            }
            else if (node is OrExp or)
            {
                foreach (var subNode in or)
                    if (IsNodeTrue(subNode, isNegative))
                        return true;
            }
            else if (node is WhenExp when)
            {
                if (IsNodeTrue(when.Condition, isNegative))
                    return IsNodeTrue(when.Effect, isNegative);
            }
            else if (node is ExistsExp exist)
            {
                var permutations = GenerateParameterPermutations(exist.Expression, exist.Parameters.Values, 0);
                foreach (var permutation in permutations)
                    if (IsNodeTrue(permutation, isNegative))
                        return true;
                return false;
            }
            else if (node is ForAllExp all)
            {
                var permutations = GenerateParameterPermutations(all.Expression, all.Parameters.Values, 0);
                foreach (var permutation in permutations)
                    if (!IsNodeTrue(permutation, isNegative))
                        return false;
                return true;
            }
            else if (node is IWalkable walk)
            {
                foreach (var subNode in walk)
                    if (!IsNodeTrue(subNode, isNegative))
                        return false;
            }
            return true;
        }

        public bool IsInGoal()
        {
            if (Declaration.Problem.Goal == null)
                throw new ArgumentNullException("No problem goal was declared!");
            return IsNodeTrue(Declaration.Problem.Goal.GoalExp);
        }
    }
}
