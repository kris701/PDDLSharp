using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class LiteralExp : BaseNode, IExp
    {
        public int Value { get; set; }

        public LiteralExp(ASTNode node, INode? parent, int value) : base(node, parent)
        {
            Value = value;
        }

        public LiteralExp(INode? parent, int value) : base(parent)
        {
            Value = value;
        }

        public LiteralExp(int value) : base()
        {
            Value = value;
        }

        public override bool Equals(object? obj)
        {
            if (obj is LiteralExp other)
            {
                if (!base.Equals(other)) return false;
                if (!Value.Equals(other.Value)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= Value.GetHashCode();
            return hash;
        }

        public override LiteralExp Copy(INode? newParent = null)
        {
            var newNode = new LiteralExp(new ASTNode(Line, "", ""), newParent, Value);
            newNode._metaInfo = new List<System.Reflection.PropertyInfo>(_metaInfo);
            newNode.IsHidden = IsHidden;
            return newNode;
        }
    }
}
