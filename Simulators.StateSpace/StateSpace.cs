using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.Plans;

namespace PDDLSharp.Simulators.StateSpace
{
    public class StateSpace
    {
        public PDDLDecl Declaration { get; internal set; }
        private HashSet<GroundedPredicate> _state;
        private HashSet<GroundedPredicate> _tempAdd = new HashSet<GroundedPredicate>();
        private HashSet<GroundedPredicate> _tempDel = new HashSet<GroundedPredicate>();

        public StateSpace(PDDLDecl declaration)
        {
            Declaration = declaration;
            _state = new HashSet<GroundedPredicate>();
        }

        public StateSpace(PDDLDecl declaration, InitDecl inits)
        {
            Declaration = declaration;
            _state = new HashSet<GroundedPredicate>();
            foreach (var item in inits.Predicates)
                if (item is PredicateExp predicate)
                    _state.Add(new GroundedPredicate(predicate));
        }

        public int Count => _state.Count;

        public void Add(GroundedPredicate pred) => _state.Add(pred);
        public void Del(GroundedPredicate pred) => _state.Remove(pred);

        public bool Contains(GroundedPredicate op) => _state.Contains(op);
        public bool Contains(string op, params string[] arguments) => Contains(new GroundedPredicate(op, NameExpBuilder.GetNameExpFromString(arguments, Declaration)));

        public void ExecuteNode(INode node)
        {
            _tempAdd.Clear();
            _tempDel.Clear();
            ExecuteNode(node, false);
            foreach (var item in _tempAdd)
                _state.Add(item);
            foreach (var item in _tempDel)
                _state.Remove(item);
        }
        private void ExecuteNode(INode node, bool isNegative)
        {
            if (node is PredicateExp predicate)
            {
                var op = new GroundedPredicate(predicate);
                if (isNegative)
                    _tempDel.Add(op);
                else
                    _tempAdd.Add(op);
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
        public bool IsNodeTrue(INode node, bool isNegative)
        {
            if (node is PredicateExp predicate)
            {
                var op = new GroundedPredicate(predicate);
                if (isNegative)
                    return !_state.Contains(op);
                else
                    return _state.Contains(op);
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
    }
}
