using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.Expressions
{
    public class WhenExp : BaseWalkableNode, IExp
    {
        public AndExp Condition { get; set; }
        public AndExp Effect { get; set; }

        public WhenExp(ASTNode node, INode parent, AndExp condition, AndExp effect) : base(node, parent)
        {
            Condition = condition;
            Effect = effect;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash *= Condition.GetHashCode();
            hash *= Effect.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Condition;
            yield return Effect;
        }
    }
}
