using PDDL.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDL.Tools;

namespace PDDL.Models.Expressions
{
    public class NumericExp : BaseWalkableNode, IExp, INamedNode
    {
        public string Name { get; set; }
        public IExp Arg1 { get; set; }
        public IExp Arg2 { get; set; }

        public NumericExp(ASTNode node, INode parent, string name, IExp arg1, IExp arg2) : base(node, parent)
        {
            Name = name;
            Arg1 = arg1;
            Arg2 = arg2;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + Name.GetHashCode() + Arg1.GetHashCode() + Arg2.GetHashCode();
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Arg1;
            yield return Arg2;
        }
    }
}
