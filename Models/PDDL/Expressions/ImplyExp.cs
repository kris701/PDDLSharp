﻿using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class ImplyExp : BaseWalkableNode, IExp
    {
        public IExp Antecedent { get; set; }
        public IExp Consequent { get; set; }

        public ImplyExp(ASTNode node, INode? parent, IExp antecedent, IExp consequent) : base(node, parent)
        {
            Antecedent = antecedent;
            Consequent = consequent;
        }

        public ImplyExp(INode? parent, IExp antecedent, IExp consequent) : base(parent)
        {
            Antecedent = antecedent;
            Consequent = consequent;
        }

        public ImplyExp(IExp antecedent, IExp consequent) : base()
        {
            Antecedent = antecedent;
            Consequent = consequent;
        }

        public ImplyExp(ASTNode node, INode? parent) : base(node, parent)
        {
            Antecedent = new AndExp(this, new List<IExp>());
            Consequent = new AndExp(this, new List<IExp>());
        }

        public ImplyExp(INode? parent) : base(parent)
        {
            Antecedent = new AndExp(this, new List<IExp>());
            Consequent = new AndExp(this, new List<IExp>());
        }

        public ImplyExp() : base()
        {
            Antecedent = new AndExp(this, new List<IExp>());
            Consequent = new AndExp(this, new List<IExp>());
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash ^= Antecedent.GetHashCode();
            hash ^= Consequent.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Antecedent;
            yield return Consequent;
        }

        public override ImplyExp Copy(INode? newParent = null)
        {
            var newNode = new ImplyExp(new ASTNode(Start, End, Line, "", ""), newParent);
            var newAntecedent = ((dynamic)Antecedent).Copy(newNode);
            var newConsequent = ((dynamic)Consequent).Copy(newNode);
            newNode.Antecedent = newAntecedent;
            newNode.Consequent = newConsequent;
            return newNode;
        }
    }
}
