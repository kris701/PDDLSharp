using PDDLSharp.Models.PDDL.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.SAS
{
    public class Fact
    {
        public string Name { get; }
        public string[] Arguments { get; }

        public Fact(string name, string[] arguments)
        {
            Name = name;
            Arguments = arguments;
        }

        public Fact(PredicateExp pred)
        {
            Name = pred.Name;
            var args = new List<string>();
            foreach (var arg in pred.Arguments)
                args.Add(arg.Name);
            Arguments = args.ToArray();
        }

        // The order is important!
        // Based on: https://stackoverflow.com/a/30758270
        private int _hashCache = -1;
        public override int GetHashCode()
        {
            if (_hashCache != -1)
                return _hashCache;
            const int seed = 487;
            const int modifier = 31;
            unchecked
            {
                _hashCache = Name.GetHashCode() * Arguments.Aggregate(seed, (current, item) =>
                    (current * modifier) + item.GetHashCode());
                return _hashCache;
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is Fact f)
                return f.GetHashCode() == GetHashCode();
            return false;
        }
    }
}
