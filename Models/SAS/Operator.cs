using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Models.SAS
{
    public class Operator
    {
        public string Name { get; }
        public string[] Arguments { get; }
        public HashSet<Fact> Pre { get; }
        public HashSet<Fact> Add { get; }
        public HashSet<Fact> Del { get; }

        public Operator(string name, string[] arguments, HashSet<Fact> pre, HashSet<Fact> add, HashSet<Fact> del)
        {
            Name = name;
            Arguments = arguments;
            Pre = pre;
            Add = add;
            Del = del;
        }

        private int _hashCache = -1;
        public override int GetHashCode()
        {
            if (_hashCache != -1)
                return _hashCache;

            _hashCache = Name.GetHashCode();
            foreach (var arg in Arguments)
                _hashCache ^= arg.GetHashCode();
            foreach (var pre in Pre)
                _hashCache ^= pre.GetHashCode();
            foreach (var del in Del)
                _hashCache ^= del.GetHashCode();
            foreach (var add in Add)
                _hashCache ^= add.GetHashCode();

            return _hashCache;
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
