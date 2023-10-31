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

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= Value.GetHashCode();
            return hash;
        }

        public override LiteralExp Copy(INode? newParent = null)
        {
            var newNode = new LiteralExp(new ASTNode(Start, End, Line, "", ""), newParent, Value);
            newNode.IsHidden = IsHidden;
            return newNode;
        }
    }
}
