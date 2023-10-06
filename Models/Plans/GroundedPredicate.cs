using PDDLSharp.Models.PDDL.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.Plans
{
    public class GroundedPredicate
    {
        public string Name { get; set; }
        public List<NameExp> Arguments { get; set; }

        public GroundedPredicate(string name, params string[] arguments)
        {
            Name = name;
            Arguments = new List<NameExp>();
            foreach (var arg in arguments)
                Arguments.Add(new NameExp(null, arg));
        }

        public GroundedPredicate(string name, List<NameExp> overrideArguments)
        {
            Name = name;
            Arguments = overrideArguments;
        }

        public GroundedPredicate(PredicateExp predicate)
        {
            Name = predicate.Name;
            Arguments = new List<NameExp>();
            foreach (var arg in predicate.Arguments)
                Arguments.Add(new NameExp(arg));
        }

        public GroundedPredicate(PredicateExp predicate, List<NameExp> overrideArguments)
        {
            Name = predicate.Name;
            Arguments = overrideArguments;
        }

        public GroundedPredicate(PredicateExp predicate, params string[] overrideArguments)
        {
            Name = predicate.Name;
            Arguments = new List<NameExp>();
            foreach (var arg in overrideArguments)
                Arguments.Add(new NameExp(arg));
        }


        public override string? ToString()
        {
            var retStr = Name;
            foreach (var arg in Arguments)
                retStr += $" {arg}";
            return retStr;
        }

        public override bool Equals(object? obj)
        {
            if (obj is GroundedPredicate op)
                return op.GetHashCode() == GetHashCode();
            return false;
        }

        public override int GetHashCode()
        {
            var hash = Name.GetHashCode();
            foreach (var arg in Arguments)
                hash ^= arg.GetHashCode();
            return hash;
        }
    }
}
