using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.PDDL.Problem
{
    public class DomainNameRefDecl : BaseNamedNode, IDecl
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
    }
}
