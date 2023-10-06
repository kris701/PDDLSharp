using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
