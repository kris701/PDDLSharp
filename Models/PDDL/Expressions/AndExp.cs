using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class AndExp : BaseWalkableNode<AndExp>, IExp
    {
        public List<IExp> Children { get; set; }

        public AndExp(ASTNode node, INode parent, List<IExp> children) : base(node, parent)
        {
            Children = children;
        }

        public AndExp(INode parent, List<IExp> children) : base(parent)
        {
            Children = children;
        }

        public AndExp(List<IExp> children) : base()
        {
            Children = children;
        }

        public AndExp() : base()
        {
            Children = new List<IExp>();
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var child in Children)
                hash *= child.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        public override AndExp Copy(INode newParent)
        {
            var newNode = new AndExp(new ASTNode(Start, End, Line, "", ""), newParent, new List<IExp>());
            foreach (var node in Children)
                newNode.Children.Add(((dynamic)node).Copy(newNode));
            return newNode;
        }
    }
}
