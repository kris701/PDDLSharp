using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PDDLSharp.Tools;
using PDDLSharp.Models.PDDL;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class AndExp : BaseWalkableNode, IExp
    {
        public List<IExp> Children { get; set; }

        public AndExp(ASTNode node, INode parent, List<IExp> children) : base(node, parent)
        {
            Children = children;
        }

        public AndExp(INode parent, List<IExp> children) : base(parent)
        {
            Children = children;
        }

        public AndExp(List<IExp> children) : base()
        {
            Children = children;
        }

        public AndExp() : base()
        {
            Children = new List<IExp>();
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var child in Children)
                hash *= child.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Children.GetEnumerator();
        }
    }
}
