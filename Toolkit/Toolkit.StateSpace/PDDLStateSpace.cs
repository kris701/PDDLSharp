using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Toolkit.Grounders;
using System.Xml.Linq;

namespace PDDLSharp.Toolkit.StateSpace
{
    public class PDDLStateSpace : IState
    {
        public PDDLDecl Declaration { get; internal set; }
        public HashSet<PredicateExp> State { get; internal set; }
        internal List<PredicateExp> _tempAdd = new List<PredicateExp>();
        internal List<PredicateExp> _tempDel = new List<PredicateExp>();
        internal ActionGrounder? _grounder;

        public PDDLStateSpace(PDDLDecl declaration)
        {
            Declaration = declaration;
            State = new HashSet<PredicateExp>();
            if (declaration.Problem.Init != null)
                foreach (var item in declaration.Problem.Init.Predicates)
                    if (item is PredicateExp predicate)
                        Add(SimplifyPredicate(predicate));
        }

        public PDDLStateSpace(PDDLDecl declaration, HashSet<PredicateExp> currentState)
        {
            Declaration = declaration;
            State = currentState;
        }

        public IState Copy()
        {
            PredicateExp[] newState = new PredicateExp[State.Count];
            State.CopyTo(newState);
            return new PDDLStateSpace(Declaration, newState.ToHashSet());
        }

        public int Count => State.Count;

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

        public void Add(PredicateExp pred) => State.Add(SimplifyPredicate(pred));
        public void Add(string pred, params string[] arguments) => Add(SimplifyPredicate(pred, arguments));
        public void Del(PredicateExp pred) => State.Remove(SimplifyPredicate(pred));
        public void Del(string pred, params string[] arguments) => Del(SimplifyPredicate(pred, arguments));
        public bool Contains(PredicateExp pred) => State.Contains(SimplifyPredicate(pred));
        public bool Contains(string pred, params string[] arguments) => Contains(SimplifyPredicate(pred, arguments));

        public override bool Equals(object? obj)
        {
            if (obj is IState other)
                foreach (var item in State)
                    if (!other.State.Contains(item))
                        return false;
            return true;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach(var item in State)
                hash ^= item.GetHashCode();
            return hash;
        }

        public virtual void ExecuteNode(INode node)
        {
            _tempAdd.Clear();
            _tempDel.Clear();
            ExecuteNode(node, false);
            foreach (var item in _tempDel)
                Del(item);
            foreach (var item in _tempAdd)
                Add(item);
        }
        internal void ExecuteNode(INode node, bool isNegative)
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
                    (x) =>
                    {
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
                        (x) =>
                        {
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
                        (x) =>
                        {
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

        public int IsTrueCount(INode node)
        {
            switch (node)
            {
                case PredicateExp predicate:
                    // Handle Equality predicate
                    if (predicate.Name == "=" && predicate.Arguments.Count == 2)
                        if (predicate.Arguments[0].Name == predicate.Arguments[1].Name)
                            return 1;
                    if (Contains(predicate))
                        return 1;
                    return 0;
                case NotExp not:
                    return IsTrueCount(not.Child);
                case OrExp or:
                    int count = 0;
                    foreach (var subNode in or)
                        count += IsTrueCount(subNode);
                    return count;
                case AndExp and:
                    int count2 = 0;
                    foreach (var subNode in and)
                        count2 += IsTrueCount(subNode);
                    return count2;
            }

            throw new Exception($"Unknown node type to evaluate! '{node.GetType()}'");
        }

        private bool CheckPermutationsStepwise(INode node, ParameterExp parameters, Func<INode, bool?> stopFunc, bool defaultReturn = true)
        {
            if (_grounder == null)
                _grounder = new ActionGrounder(Declaration);
            var allPermuations = _grounder.GenerateParameterPermutations(parameters.Values);
            for (int i = 0; i < allPermuations.Count; i++)
            {
                var res = stopFunc(GenerateNewParametized(node, parameters, allPermuations[i]));
                if (res != null)
                    return (bool)res;
            }
            return defaultReturn;
        }

        private INode GenerateNewParametized(INode node, ParameterExp replace, List<string> with)
        {
            var checkNode = node.Copy();
            for (int i = 0; i < replace.Values.Count; i++)
            {
                var allRefs = checkNode.FindNames(replace.Values[i].Name);
                foreach (var name in allRefs)
                    name.Name = with[i];
            }

            return checkNode;
        }

        public bool IsInGoal()
        {
            if (Declaration.Problem.Goal == null)
                throw new ArgumentNullException("No problem goal was declared!");
            return IsNodeTrue(Declaration.Problem.Goal.GoalExp);
        }
    }
}
