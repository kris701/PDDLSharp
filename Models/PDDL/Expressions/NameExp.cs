using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class NameExp : BaseNamedNode, IExp
    {
        public TypeExp Type { get; set; }

        public NameExp(ASTNode node, INode? parent, string name, TypeExp type) : base(node, parent, name)
        {
            Type = type;
        }

        public NameExp(INode? parent, string name, TypeExp type) : base(parent, name)
        {
            Type = type;
        }

        public NameExp(string name, TypeExp type) : base(name)
        {
            Type = type;
        }

        public NameExp(ASTNode node, INode? parent, string name) : base(node, parent, name)
        {
            Type = new TypeExp(node, this, "object");
        }

        public NameExp(INode? parent, string name) : base(parent, name)
        {
            Type = new TypeExp(this, "object");
        }

        public NameExp(string name) : base(name)
        {
            Type = new TypeExp(this, "object");
        }

        public override bool Equals(object? obj)
        {
            if (obj is NameExp other)
            {
                if (!base.Equals(other)) return false;
                if (!Type.Equals(other.Type)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Type.GetHashCode();
        }

        public override NameExp Copy(INode? newParent = null)
        {
            var newNode = new NameExp(new ASTNode(Line, "", ""), newParent, Name);
            newNode._metaInfo = new List<System.Reflection.PropertyInfo>(_metaInfo);
            newNode.Type = Type.Copy(newNode);
            newNode.IsHidden = IsHidden;
            return newNode;
        }

        public override void RemoveContext()
        {
            base.RemoveContext();
            Type.RemoveContext();
        }

        public override void RemoveTypes()
        {
            Type = new TypeExp("object");
        }
    }
}
