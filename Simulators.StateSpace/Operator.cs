using PDDLSharp.Models.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Simulators.StateSpace
{
    public class Operator
    {
        public string Name { get; set; }
        public List<OperatorObject> Arguments { get; set; }

        public Operator(string name, List<OperatorObject> arguments)
        {
            Name = name.ToLower();
            Arguments = arguments;
        }

        public Operator(string name, params string[] arguments)
        {
            Name = name.ToLower();
            Arguments = new List<OperatorObject>();
            foreach(var arg in arguments)
                Arguments.Add(new OperatorObject(arg));
        }

        public Operator(PredicateExp predicate)
        {
            Name = predicate.Name.ToLower();
            Arguments = new List<OperatorObject>();
            foreach (var arg in predicate.Arguments)
                Arguments.Add(new OperatorObject(arg));
        }

        public override string? ToString()
        {
            var retStr = Name;
            foreach(var arg in Arguments)
                retStr += $" {arg}";
            return retStr;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Operator op)
                return op.GetHashCode() == GetHashCode();
            return false;
        }

        public override int GetHashCode()
        {
            var hash = Name.GetHashCode();
            foreach(var arg in Arguments)
                hash ^= arg.GetHashCode();
            return hash;
        }
    }
}
