using PDDLSharp.Models.AST;
using PDDLSharp.Tools;

namespace PDDLSharp.Models.FastDownward.SAS.Sections
{
    public class InitStateDecl : BaseSASNode
    {
        public List<int> Inits { get; set; }

        public InitStateDecl(ASTNode node, List<int> inits) : base(node)
        {
            Inits = inits;
        }

        public InitStateDecl(List<int> inits)
        {
            Inits = inits;
        }

        public override string? ToString()
        {
            var retStr = "";
            foreach (var value in Inits)
                retStr += $"{value} ";
            return retStr.Trim();
        }

        public override bool Equals(object? obj)
        {
            if (obj is InitStateDecl other)
            {
                if (!EqualityHelper.AreListsEqual(Inits, other.Inits)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hash = 1;
            foreach (var child in Inits)
                hash ^= child.GetHashCode();
            return hash;
        }
    }
}
