using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.Expressions
{
    public class TimedLiteralExp : BaseWalkableNode, IExp
    {
        public int Value { get; set; }
        public IExp Literal { get; set; }

        public TimedLiteralExp(ASTNode node, INode? parent, int value, IExp literal) : base(node, parent)
        {
            Value = value;
            Literal = literal;
        }

        public TimedLiteralExp(INode? parent, int value, IExp literal) : base(parent)
        {
            Value = value;
            Literal = literal;
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash *= Value.GetHashCode();
            hash *= Literal.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Literal;
        }
    }
}
