using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class DomainNameDecl : BaseNamedNode, IDecl
    {
        public DomainNameDecl(ASTNode node, INode? parent, string name) : base(node, parent, name)
        {
        }

        public DomainNameDecl(INode? parent, string name) : base(parent, name)
        {
        }

        public DomainNameDecl(string name) : base(name)
        {
        }

        public override bool Equals(object? obj)
        {
            if (obj is DomainNameDecl other)
            {
                if (!base.Equals(other)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override DomainNameDecl Copy(INode? newParent = null)
        {
            var newNode = new DomainNameDecl(new ASTNode(Line, "", ""), newParent, Name);
            newNode._metaInfo = new List<System.Reflection.PropertyInfo>(_metaInfo);
            newNode.IsHidden = IsHidden;
            return newNode;
        }
    }
}
