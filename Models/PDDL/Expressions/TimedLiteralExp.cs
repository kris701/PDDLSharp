﻿using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Expressions
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

        public TimedLiteralExp(int value, IExp literal) : base()
        {
            Value = value;
            Literal = literal;
        }

        public TimedLiteralExp(ASTNode node, INode? parent, int value) : base(node, parent)
        {
            Value = value;
            Literal = new AndExp(this, new List<IExp>());
        }

        public TimedLiteralExp(INode? parent, int value) : base(parent)
        {
            Value = value;
            Literal = new AndExp(this, new List<IExp>());
        }

        public TimedLiteralExp(int value) : base()
        {
            Value = value;
            Literal = new AndExp(this, new List<IExp>());
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= Value.GetHashCode();
            hash ^= Literal.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Literal;
        }

        public override TimedLiteralExp Copy(INode? newParent = null)
        {
            var newNode = new TimedLiteralExp(new ASTNode(Start, End, Line, "", ""), newParent, Value);
            newNode.Literal = ((dynamic)Literal).Copy(newNode);
            newNode.IsHidden = IsHidden;
            return newNode;
        }

        public override void Replace(INode node, INode with)
        {
            if (Literal == node && with is IExp exp1)
                Literal = exp1;
        }
    }
}
