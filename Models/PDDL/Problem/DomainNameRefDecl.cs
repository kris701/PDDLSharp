using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Tools;

namespace PDDLSharp.Models.PDDL.Problem
{
    public class DomainNameRefDecl : BaseNamedNode, IDecl
    {

        public DomainNameRefDecl(ASTNode node, INode? parent, string name) : base(node, parent, name)
        {
        }

        public DomainNameRefDecl(INode? parent, string name) : base(parent, name)
        {
        }

        public DomainNameRefDecl(string name) : base(name)
        {
        }

        public override bool Equals(object? obj)
        {
            if (obj is DomainNameRefDecl other)
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

        public override DomainNameRefDecl Copy(INode? newParent = null)
        {
            var newNode = new DomainNameRefDecl(new ASTNode(Start, End, Line, "", ""), newParent, Name);
            newNode.IsHidden = IsHidden;
            return newNode;
        }
    }
}
