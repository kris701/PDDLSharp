using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class TimedLiteralExp : BaseWalkableNode, IExp
    {
        public int Value { get; set; }
        public IExp Literal { get; set; }

        public TimedLiteralExp(ASTNode node, INode parent, int value, IExp literal) : base(node, parent)
        {
            Value = value;
            Literal = literal;
        }

        public TimedLiteralExp(INode parent, int value, IExp literal) : base(parent)
        {
            Value = value;
            Literal = literal;
        }

        public TimedLiteralExp(int value, IExp literal) : base()
        {
            Value = value;
            Literal = literal;
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash *= Value.GetHashCode();
            hash *= Literal.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Literal;
        }
    }
}
