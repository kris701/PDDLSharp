using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Toolkit.Grounders
{
    public class PredicateGrounder : BaseGrounder<PredicateExp>
    {
        public PredicateGrounder(PDDLDecl declaration) : base(declaration)
        {
        }

        public override List<PredicateExp> Ground(PredicateExp item)
        {
            List<PredicateExp> newPredicates = new List<PredicateExp>();

            if (item.Arguments.Count == 0)
                return new List<PredicateExp>() { item.Copy() };

            var allPermuations = GenerateParameterPermutations(item.Arguments);
            foreach (var premutation in allPermuations)
            {
                List<NameExp> arguments = new List<NameExp>();
                foreach (var arg in premutation)
                    arguments.Add(new NameExp(GetObjectFromIndex(arg).Name));
                newPredicates.Add(new PredicateExp(item.Name, arguments));
            }

            return newPredicates;
        }
    }
}
