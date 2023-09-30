using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Tools;
using PDDLSharp.Models.Expressions;

namespace PDDLSharp.Models.Domain
{
    public class FunctionsDecl : BaseWalkableNode, IDecl
    {
        public List<PredicateExp> Functions { get; set; }

        public FunctionsDecl(ASTNode node, INode? parent, List<PredicateExp> functions) : base(node, parent)
        {
            Functions = functions;
        }

        public FunctionsDecl(INode? parent, List<PredicateExp> functions) : base(parent)
        {
            Functions = functions;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var pred in Functions)
                hash *= pred.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Functions.GetEnumerator();
        }
    }
}
