using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.Plans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Grounders
{
    public class PredicateGrounder : BaseGrounder<PredicateExp, PredicateExp>
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
                    arguments.Add(new NameExp(arg));
                newPredicates.Add(new PredicateExp(item.Name, arguments));
            }

            return newPredicates;
        }
    }
}
