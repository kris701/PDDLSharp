using PDDLSharp.Models.AST;
using PDDLSharp.Tools;

namespace PDDLSharp.Models.FastDownward.SAS.Sections
{
    public class OperatorDecl : BaseSASNode
    {
        public string Name { get; set; }
        public List<ValuePair> PrevailConditions { get; set; }
        public List<OperatorEffect> Effects { get; set; }
        public int Cost { get; set; }

        public OperatorDecl(ASTNode node, string name, List<ValuePair> prevailConditions, List<OperatorEffect> effects, int cost) : base(node)
        {
            Name = name;
            PrevailConditions = prevailConditions;
            Effects = effects;
            Cost = cost;
        }

        public OperatorDecl(string name, List<ValuePair> prevailConditions, List<OperatorEffect> effects, int cost)
        {
            Name = name;
            PrevailConditions = prevailConditions;
            Effects = effects;
            Cost = cost;
        }

        public override string? ToString()
        {
            return $"{Name}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is OperatorDecl other)
            {
                if (Name != other.Name) return false;
                if (Cost != other.Cost) return false;
                if (!EqualityHelper.AreListsEqual(PrevailConditions, other.PrevailConditions)) return false;
                if (!EqualityHelper.AreListsEqual(Effects, other.Effects)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hash = Name.GetHashCode() ^ Cost.GetHashCode();
            foreach (var child in PrevailConditions)
                hash ^= child.GetHashCode();
            foreach (var child in Effects)
                hash ^= child.GetHashCode();
            return hash;
        }
    }
}
