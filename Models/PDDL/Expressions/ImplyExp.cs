using PDDLSharp.Models.AST;
using System.Linq.Expressions;

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

        public override bool Equals(object? obj)
        {
            if (obj is ImplyExp other)
            {
                if (!base.Equals(other)) return false;
                if (!Antecedent.Equals(other.Antecedent)) return false;
                if (!Consequent.Equals(other.Consequent)) return false;
                return true;
            }
            return false;
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
            var newNode = new ImplyExp(new ASTNode(Line, "", ""), newParent);
            var newAntecedent = ((dynamic)Antecedent).Copy(newNode);
            var newConsequent = ((dynamic)Consequent).Copy(newNode);
            newNode.Antecedent = newAntecedent;
            newNode.Consequent = newConsequent;
            newNode.IsHidden = IsHidden;
            return newNode;
        }

        public override void Replace(INode node, INode with)
        {
            if (Antecedent == node && with is IExp exp1)
                Antecedent = exp1;
            else if (Consequent == node && with is IExp exp2)
                Consequent = exp2;
        }
    }
}
