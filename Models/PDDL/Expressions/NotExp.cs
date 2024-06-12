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

        public override bool Equals(object? obj)
        {
            if (obj is NotExp other)
            {
                if (!base.Equals(other)) return false;
                if (!Child.Equals(other.Child)) return false;
                return true;
            }
            return false;
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
            var newNode = new NotExp(new ASTNode(Line, "", ""), newParent);
            newNode._metaInfo = new List<System.Reflection.PropertyInfo>(_metaInfo);
            newNode.Child = ((dynamic)Child).Copy(newNode);
            newNode.IsHidden = IsHidden;
            return newNode;
        }

        public override void Replace(INode node, INode with)
        {
            if (Child == node && with is IExp exp)
                Child = exp;
        }
    }
}
