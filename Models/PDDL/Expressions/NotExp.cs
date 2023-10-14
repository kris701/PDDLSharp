using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class NotExp : BaseWalkableNode, IExp
    {
        public IExp Child { get; set; }

        public NotExp(ASTNode node, INode? parent, IExp child) : base(node, parent)
        {
            Child = child;
        }

        public NotExp(INode? parent, IExp child) : base(parent)
        {
            Child = child;
        }

        public NotExp(IExp child) : base()
        {
            Child = child;
        }

        public NotExp(ASTNode node, INode? parent) : base(node, parent)
        {
            Child = new AndExp(this, new List<IExp>());
        }

        public NotExp(INode? parent) : base(parent)
        {
            Child = new AndExp(this, new List<IExp>());
        }

        public NotExp() : base()
        {
            Child = new AndExp(this, new List<IExp>());
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Child.GetHashCode();
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Child;
        }

        public override NotExp Copy(INode? newParent = null)
        {
            var newNode = new NotExp(new ASTNode(Start, End, Line, "", ""), newParent);
            newNode.Child = ((dynamic)Child).Copy(newNode);
            return newNode;
        }

        public override void Replace(INode node, INode with)
        {
            if (Child == node && with is IExp exp)
                Child = exp;
        }
    }
}
