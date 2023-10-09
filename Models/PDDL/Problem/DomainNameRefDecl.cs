using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Problem
{
    public class DomainNameRefDecl : BaseNamedNode<DomainNameRefDecl>, IDecl
    {

        public DomainNameRefDecl(ASTNode node, INode parent, string name) : base(node, parent, name)
        {
        }

        public DomainNameRefDecl(INode parent, string name) : base(parent, name)
        {
        }

        public DomainNameRefDecl(string name) : base(name)
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override DomainNameRefDecl Copy(INode newParent)
        {
            return new DomainNameRefDecl(new ASTNode(Start, End, Line, "", ""), newParent, Name);
        }
    }
}
