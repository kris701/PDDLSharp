using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class ImplyExp : BaseWalkableNode<ImplyExp>, IExp
    {
        public IExp Antecedent { get; set; }
        public IExp Consequent { get; set; }

        public ImplyExp(ASTNode node, INode parent, IExp antecedent, IExp consequent) : base(node, parent)
        {
            Antecedent = antecedent;
            Consequent = consequent;
        }

        public ImplyExp(INode parent, IExp antecedent, IExp Consequent) : base(parent)
        {
            Antecedent = antecedent;
            this.Consequent = Consequent;
        }

        public ImplyExp(IExp antecedent, IExp Consequent) : base()
        {
            Antecedent = antecedent;
            this.Consequent = Consequent;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash *= Antecedent.GetHashCode();
            hash *= Consequent.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Antecedent;
            yield return Consequent;
        }

        public override ImplyExp Copy(INode newParent)
        {
            var newNode = new ImplyExp(new ASTNode(Start, End, Line, "", ""), newParent, null, null);
            var newAntecedent = ((dynamic)Antecedent).Copy(newNode);
            var newConsequent = ((dynamic)Consequent).Copy(newNode);
            newNode.Antecedent = newAntecedent;
            newNode.Consequent = newConsequent;
            return newNode;
        }
    }
}
