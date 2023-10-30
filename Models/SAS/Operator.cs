using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.SAS
{
    public class Operator
    {
        public string Name { get; }
        public string[] Arguments { get; }
        public Fact[] Pre { get; }
        public Fact[] Add { get; }
        public Fact[] Del { get; }

        public Operator(string name, string[] arguments, Fact[] pre, Fact[] add, Fact[] del)
        {
            Name = name;
            Arguments = arguments;
            Pre = pre;
            Add = add;
            Del = del;
        }

        public Operator(PDDL.Domain.ActionDecl act)
        {
            Name = act.Name;
            // Arguments
            var args = new List<string>();
            foreach (var arg in act.Parameters.Values)
                args.Add(arg.Name);
            Arguments = args.ToArray();

            // Preconditions
            var pre = new List<Fact>();
            if (act.Preconditions is AndExp preAnd)
            {
                foreach(var item in preAnd.Children)
                {
                    if (item is PredicateExp pred)
                        pre.Add(new Fact(pred));
                    else
                        throw new ArgumentException("Unsupported node for operator!");
                }
            }
            else
                throw new ArgumentException("Action precondition must be an and expression.");
            Pre = pre.ToArray();

            // Effects
            var add = new List<Fact>();
            var del = new List<Fact>();
            if (act.Effects is AndExp effAnd)
            {
                foreach (var item in effAnd.Children)
                {
                    if (item is PredicateExp pred)
                        add.Add(new Fact(pred));
                    else
                    {
                        if (item is NotExp not && not.Child is PredicateExp nPred)
                            del.Add(new Fact(nPred));
                        else
                            throw new ArgumentException("Unsupported node for operator!");
                    }
                }
            }
            else
                throw new ArgumentException("Action effect must be an and expression.");
            Add = add.ToArray();
            Del = del.ToArray();
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
                    (current * modifier) + item.GetHashCode()) * Pre.Aggregate(seed, (current, item) =>
                    (current * modifier) + item.GetHashCode()) * Add.Aggregate(seed, (current, item) =>
                    (current * modifier) + item.GetHashCode()) * Del.Aggregate(seed, (current, item) =>
                    (current * modifier) + item.GetHashCode());
                return _hashCache;
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is Operator o)
                return o.GetHashCode() == GetHashCode();
            return false;
        }

        public override string? ToString()
        {
            var retStr = Name;
            foreach (var arg in Arguments)
                retStr += $" {arg}";
            return retStr;
        }
    }
}
