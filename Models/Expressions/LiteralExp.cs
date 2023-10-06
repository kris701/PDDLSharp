using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.Expressions
{
    public class LiteralExp : BaseNode, IExp
    {
        public int Value { get; set; }

        public LiteralExp(ASTNode node, INode parent, int value) : base(node, parent)
        {
            Value = value;
        }

        public LiteralExp(INode parent, int value) : base(parent)
        {
            Value = value;
        }

        public LiteralExp(int value) : base()
        {
            Value = value;
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash *= Value.GetHashCode();
            return hash;
        }
    }
}
