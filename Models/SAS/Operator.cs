namespace PDDLSharp.Models.SAS
{
    public class Operator
    {
        public int ID { get; set; } = -1;
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

        public Operator()
        {
            Name = "";
            Arguments = new string[0];
            Pre = new Fact[0];
            Add = new Fact[0];
            Del = new Fact[0];
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
                if (ID != o.ID)
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
            var pre = new Fact[Pre.Length];
            var add = new Fact[Add.Length];
            var del = new Fact[Del.Length];

            for (int i = 0; i < Arguments.Length; i++)
                arguments[i] = Arguments[i];
            for (int i = 0; i < Pre.Length; i++)
                pre[i] = Pre[i].Copy();
            for (int i = 0; i < Add.Length; i++)
                add[i] = Add[i].Copy();
            for (int i = 0; i < Del.Length; i++)
                del[i] = Del[i].Copy();

            var newOp = new Operator(Name, arguments, pre, add, del);
            newOp.ID = ID;
            return newOp;
        }
    }
}
