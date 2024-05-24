using PDDLSharp.Tools;

namespace PDDLSharp.Models.SAS
{
    public class SASDecl
    {
        public HashSet<string> DomainVariables { get; set; }
        public List<Operator> Operators { get; set; }
        public HashSet<Fact> Goal { get; set; }
        public HashSet<Fact> Init { get; set; }

        private Dictionary<int, Operator> _operatorDict;
        private Dictionary<int, Fact> _factDict;

        public SASDecl(HashSet<string> domainVariables, List<Operator> operators, HashSet<Fact> goal, HashSet<Fact> init)
        {
            DomainVariables = domainVariables;
            Operators = operators;
            Goal = goal;
            Init = init;
            _operatorDict = new Dictionary<int, Operator>();
            _factDict = new Dictionary<int, Fact>();

            foreach(var op in operators)
            {
                if (_operatorDict.ContainsKey(op.ID))
                    continue;
                _operatorDict.Add(op.ID, op);

                foreach(var pre in op.Pre)
                {
                    if (_factDict.ContainsKey(pre.ID))
                        continue;
                    _factDict.Add(pre.ID, pre);
                }
                foreach (var add in op.Add)
                {
                    if (_factDict.ContainsKey(add.ID))
                        continue;
                    _factDict.Add(add.ID, add);
                }
                foreach (var del in op.Del)
                {
                    if (_factDict.ContainsKey(del.ID))
                        continue;
                    _factDict.Add(del.ID, del);
                }
            }
        }

        public SASDecl() : this(new HashSet<string>(), new List<Operator>(), new HashSet<Fact>(), new HashSet<Fact>())
        {
        }

        public SASDecl Copy()
        {
            var domainVariables = new HashSet<string>();
            var operators = new List<Operator>();
            var goal = new HashSet<Fact>();
            var init = new HashSet<Fact>();

            foreach (var domainVar in DomainVariables)
                domainVariables.Add(domainVar);
            foreach (var op in Operators)
                operators.Add(op.Copy());
            foreach (var g in Goal)
                goal.Add(g.Copy());
            foreach (var i in Init)
                init.Add(i.Copy());

            return new SASDecl(domainVariables, operators, goal, init);
        }

        public override bool Equals(object? obj)
        {
            if (obj is SASDecl other)
            {
                if (!EqualityHelper.AreListsEqual(DomainVariables, other.DomainVariables)) return false;
                if (!EqualityHelper.AreListsEqual(Operators, other.Operators)) return false;
                if (!EqualityHelper.AreListsEqual(Goal, other.Goal)) return false;
                if (!EqualityHelper.AreListsEqual(Init, other.Init)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hash = 1;
            foreach (var child in DomainVariables)
                hash ^= child.GetHashCode();
            foreach (var child in Operators)
                hash ^= child.GetHashCode();
            foreach (var child in Goal)
                hash ^= child.GetHashCode();
            foreach (var child in Init)
                hash ^= child.GetHashCode();
            return hash;
        }

        public Operator GetOperatorByID(int id) => _operatorDict[id];
        public Fact GetFactByID(int id) => _factDict[id];
    }
}
