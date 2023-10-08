using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class DomainNameDecl : BaseNamedNode, IDecl
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
    }
}
