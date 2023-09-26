using PDDL.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDL.Tools;

namespace PDDL.Models.Problem
{
    public class InitDecl : BaseWalkableNode, IDecl
    {
        public List<IExp> Predicates { get; set; }

        public InitDecl(ASTNode node, INode parent, List<IExp> predicates) : base(node, parent)
        {
            Predicates = predicates;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach(var pred in Predicates)
                hash *= pred.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Predicates.GetEnumerator();
        }
    }
}
