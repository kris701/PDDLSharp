using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PDDLSharp.Tools;
using PDDLSharp.Models.Expressions;

namespace PDDLSharp.Models.Domain
{
    public class PredicatesDecl : BaseWalkableNode, IDecl
    {
        public List<PredicateExp> Predicates { get; set; }

        public PredicatesDecl(ASTNode node, INode? parent, List<PredicateExp> predicates) : base(node, parent)
        {
            Predicates = predicates;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var pred in Predicates)
                hash *= pred.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Predicates.GetEnumerator();
        }
    }
}
