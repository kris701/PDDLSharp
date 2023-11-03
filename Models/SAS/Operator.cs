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

        public Operator()
        {
            Name = "";
            Arguments = new string[0];
            Pre = new HashSet<Fact>();
            Add = new HashSet<Fact>();
            Del = new HashSet<Fact>();
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
            {
                if (Name != o.Name) return false;
                if (Arguments.Length != o.Arguments.Length) return false;
                for (int i = 0; i < Arguments.Length; i++)
                    if (Arguments[i] != o.Arguments[i])
                        return false;
                foreach (var pre in Pre)
                    if (!o.Pre.Contains(pre))
                        return false;
                foreach (var add in Add)
                    if (!o.Add.Contains(add))
                        return false;
                foreach (var del in Del)
                    if (!o.Del.Contains(del))
                        return false;
                return true;
            }
            return false;
        }

        public override string? ToString()
        {
            var retStr = Name;
            foreach (var arg in Arguments)
                retStr += $" {arg}";
            return retStr;
        }

        public Operator Copy()
        {
            var arguments = new string[Arguments.Length];
            var pre = new HashSet<Fact>();
            var add = new HashSet<Fact>();
            var del = new HashSet<Fact>();

            for (int i = 0; i < Arguments.Length; i++)
                arguments[i] = Arguments[i];
            foreach (var p in Pre)
                pre.Add(p.Copy());
            foreach (var a in Add)
                add.Add(a.Copy());
            foreach (var d in Del)
                del.Add(d.Copy());

            return new Operator(Name, arguments, pre, add, del);
        }
    }
}
