using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.Problem
{
    public class ProblemNameDecl : BaseNamedNode, IDecl
    {
        public ProblemNameDecl(ASTNode node, INode? parent, string name) : base(node, parent, name)
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
