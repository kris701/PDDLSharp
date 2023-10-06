using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.PDDL.Problem
{
    public class ProblemNameDecl : BaseNamedNode, IDecl
    {
        public ProblemNameDecl(ASTNode node, INode parent, string name) : base(node, parent, name)
        {
        }

        public ProblemNameDecl(INode parent, string name) : base(parent, name)
        {
        }

        public ProblemNameDecl(string name) : base(name)
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
