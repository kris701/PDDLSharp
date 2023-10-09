using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class NotExp : BaseWalkableNode<NotExp>, IExp
    {
        public IExp Child { get; set; }

        public NotExp(ASTNode node, INode parent, IExp child) : base(node, parent)
        {
            Child = child;
        }

        public NotExp(INode parent, IExp child) : base(parent)
        {
            Child = child;
        }

        public NotExp(IExp child) : base()
        {
            Child = child;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() * Child.GetHashCode();
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Child;
        }

        public override NotExp Copy(INode newParent)
        {
            var newNode = new NotExp(new ASTNode(Start, End, Line, "", ""), newParent, null);
            newNode.Child = ((dynamic)Child).Copy(newNode);
            return newNode;
        }
    }
}
