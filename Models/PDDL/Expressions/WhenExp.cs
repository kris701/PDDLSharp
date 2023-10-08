﻿using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class WhenExp : BaseWalkableNode, IExp
    {
        public IExp Condition { get; set; }
        public IExp Effect { get; set; }

        public WhenExp(ASTNode node, INode parent, IExp condition, IExp effect) : base(node, parent)
        {
            Condition = condition;
            Effect = effect;
        }

        public WhenExp(INode parent, IExp condition, IExp effect) : base(parent)
        {
            Condition = condition;
            Effect = effect;
        }

        public WhenExp(IExp condition, IExp effect) : base()
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