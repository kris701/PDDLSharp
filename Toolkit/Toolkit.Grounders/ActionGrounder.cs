using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Toolkit.StaticPredicateDetectors;

namespace PDDLSharp.Toolkit.Grounders
{
    public class ActionGrounder : BaseGrounder<ActionDecl>
    {
        public ActionGrounder(PDDLDecl declaration) : base(declaration)
        {
        }

        public override List<ActionDecl> Ground(ActionDecl item)
        {
            List<ActionDecl> groundedActions = new List<ActionDecl>();

            if (item.Parameters.Values.Count == 0 && item.Copy() is ActionDecl newItem)
                return new List<ActionDecl>() { newItem };

            IStaticPredicateDetectors staticPredicateDetector = new SimpleStaticPredicateDetector();
            var statics = staticPredicateDetector.FindStaticPredicates(Declaration);

            List<PredicateExp> inits = new List<PredicateExp>();
            if (Declaration.Problem.Init != null)
                foreach (var init in Declaration.Problem.Init.Predicates)
                    if (init is PredicateExp pred)
                        inits.Add(pred);

            var allPermutations = GenerateParameterPermutations(item.Parameters.Values);
            foreach (var permutation in allPermutations)
            {
                var copy = item.Copy();
                for (int i = 0; i < item.Parameters.Values.Count; i++)
                {
                    var allRefs = copy.FindNames(item.Parameters.Values[i].Name);
                    foreach (var refItem in allRefs)
                        refItem.Name = GetObjectFromIndex(permutation[i]).Name;
                }

                if (IsStaticsValidForPermutation(copy, inits, statics))
                    groundedActions.Add(copy);
            }

            return groundedActions;
        }

        private bool IsStaticsValidForPermutation(ActionDecl action, List<PredicateExp> inits, List<PredicateExp> statics)
        {
            if (statics.Count == 0)
                return true;
            return IsNodeTrue(action.Preconditions, inits, statics);
        }

        private bool IsNodeTrue(INode node, List<PredicateExp> inits, List<PredicateExp> statics)
        {
            switch (node)
            {
                case PredicateExp predicate:
                    // Handle Equality predicate
                    if (predicate.Name == "=" && predicate.Arguments.Count == 2)
                        return predicate.Arguments[0].Name == predicate.Arguments[1].Name;

                    if (statics.Any(x => x.Name == predicate.Name))
                        return inits.Contains(predicate);
                    return true;
                case NotExp not:
                    return !IsNodeTrue(not.Child, inits, statics);
                case OrExp or:
                    foreach (var subNode in or)
                        if (IsNodeTrue(subNode, inits, statics))
                            return true;
                    return false;
                case AndExp and:
                    foreach (var subNode in and)
                        if (!IsNodeTrue(subNode, inits, statics))
                            return false;
                    return true;
            }

            throw new Exception($"Unknown node type to evaluate! '{node.GetType()}'");
        }
    }
}
