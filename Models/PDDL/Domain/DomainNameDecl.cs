using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class DomainNameDecl : BaseNamedNode<DomainNameDecl>, IDecl
    {
        public DomainNameDecl(ASTNode node, INode parent, string name) : base(node, parent, name)
        {
        }

        public DomainNameDecl(INode parent, string name) : base(parent, name)
        {
        }

        public DomainNameDecl(string name) : base(name)
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override DomainNameDecl Copy(INode newParent)
        {
            return new DomainNameDecl(new ASTNode(Start, End, Line, "", ""), newParent, Name);
        }
    }
}
