using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.Domain
{
    public class DomainNameDecl : BaseNamedNode, IDecl
    {
        public DomainNameDecl(ASTNode node, INode? parent, string name) : base(node, parent, name)
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
