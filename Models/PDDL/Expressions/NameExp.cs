using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class NameExp : BaseNamedNode, IExp
    {
        public TypeExp Type { get; set; }

        public NameExp(ASTNode node, INode parent, string name, TypeExp type) : base(node, parent, name)
        {
            Type = type;
        }

        public NameExp(INode parent, string name, TypeExp type) : base(parent, name)
        {
            Type = type;
        }

        public NameExp(string name, TypeExp type) : base(name)
        {
            Type = type;
        }

        public NameExp(ASTNode node, INode parent, string name) : base(node, parent, name)
        {
            Type = new TypeExp(node, this, "");
        }

        public NameExp(INode parent, string name) : base(parent, name)
        {
            Type = new TypeExp(this, "");
        }

        public NameExp(string name) : base(name)
        {
            Type = new TypeExp(this, "");
        }

        public NameExp(NameExp other) : base(other.Name)
        {
            Type = new TypeExp(other.Type);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + Type.GetHashCode();
        }

        public override NameExp Copy(INode newParent)
        {
            var newNode = new NameExp(new ASTNode(Start, End, Line, "", ""), newParent, Name);
            newNode.Type = Type.Copy(newNode);
            return newNode;
        }
    }
}
