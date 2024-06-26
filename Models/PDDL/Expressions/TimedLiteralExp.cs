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

        public override bool Equals(object? obj)
        {
            if (obj is TimedLiteralExp other)
            {
                if (!base.Equals(other)) return false;
                if (!Value.Equals(other.Value)) return false;
                if (!Literal.Equals(other.Literal)) return false;
                return true;
            }
            return false;
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
            var newNode = new TimedLiteralExp(new ASTNode(Line, "", ""), newParent, Value);
            newNode._metaInfo = new List<System.Reflection.PropertyInfo>(_metaInfo);
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
