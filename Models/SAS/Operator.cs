using PDDLSharp.Tools;

namespace PDDLSharp.Models.SAS
{
    public class Operator
    {
        public int ID = -1;
        public string Name;
        public string[] Arguments;
        public Fact[] Pre;
        public HashSet<int> PreRef;
        public int PreCount;
        public Fact[] Add;
        public HashSet<int> AddRef;
        public int AddCount;
        public Fact[] Del;
        public HashSet<int> DelRef;
        public int DelCount;

        public Operator(string name, string[] arguments, Fact[] pre, Fact[] add, Fact[] del)
        {
            Name = name;
            Arguments = arguments;
            Pre = pre;
            PreCount = pre.Length;
            PreRef = new HashSet<int>();
            foreach (var item in Pre)
                PreRef.Add(item.ID);
            Add = add;
            AddCount = add.Length;
            AddRef = new HashSet<int>();
            foreach (var item in Add)
                AddRef.Add(item.ID);
            Del = del;
            DelCount = del.Length;
            DelRef = new HashSet<int>();
            foreach (var item in Del)
                DelRef.Add(item.ID);
        }

        public Operator()
        {
            Name = "";
            Arguments = Array.Empty<string>();
            Pre = Array.Empty<Fact>();
            Add = Array.Empty<Fact>();
            Del = Array.Empty<Fact>();
            PreRef = new HashSet<int>();
            AddRef = new HashSet<int>();
            DelRef = new HashSet<int>();
            PreCount = 0;
            AddCount = 0;
            DelCount = 0;
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

        /// <summary>
        /// Equals is just based on the ID of the operator, since the translator only outputs unique IDs
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
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

        public bool ContentEquals(object? obj)
        {
            if (obj is Operator o)
            {
                if (Name != o.Name) return false;
                if (!EqualityHelper.AreListsEqual(Arguments, o.Arguments)) return false;
                if (PreCount != o.PreCount) return false;
                if (!Pre.All(x => o.Pre.Contains(x))) return false;
                if (AddCount != o.AddCount) return false;
                if (!Add.All(x => o.Add.Contains(x))) return false;
                if (DelCount != o.DelCount) return false;
                if (!Del.All(x => o.Del.Contains(x))) return false;
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
            var pre = new Fact[PreCount];
            var add = new Fact[AddCount];
            var del = new Fact[DelCount];

            for (int i = 0; i < Arguments.Length; i++)
                arguments[i] = Arguments[i];
            for (int i = 0; i < PreCount; i++)
                pre[i] = Pre[i].Copy();
            for (int i = 0; i < AddCount; i++)
                add[i] = Add[i].Copy();
            for (int i = 0; i < DelCount; i++)
                del[i] = Del[i].Copy();

            var newOp = new Operator(Name, arguments, pre, add, del);
            newOp.ID = ID;
            return newOp;
        }
    }
}
