using PDDLSharp.Models.AST;
using PDDLSharp.Tools;

namespace PDDLSharp.Models.FastDownward.SAS.Sections
{
    public class MutexDecl : BaseSASNode
    {
        public List<ValuePair> Group { get; set; }

        public MutexDecl(ASTNode node, List<ValuePair> group) : base(node)
        {
            Group = group;
        }

        public MutexDecl(List<ValuePair> group)
        {
            Group = group;
        }

        public override bool Equals(object? obj)
        {
            if (obj is MutexDecl other)
            {
                if (!EqualityHelper.AreListsEqual(Group, other.Group)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hash = 1;
            foreach (var child in Group)
                hash ^= child.GetHashCode();
            return hash;
        }

        public MutexDecl Copy()
        {
            var group = new List<ValuePair>();
            foreach (var gr in Group)
                group.Add(gr.Copy());
            return new MutexDecl(group);
        }
    }
}
