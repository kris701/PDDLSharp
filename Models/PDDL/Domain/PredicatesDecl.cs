using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class PredicatesDecl : BaseListableNode, IDecl
    {
        public List<PredicateExp> Predicates { get; set; }

        public PredicatesDecl(ASTNode node, INode? parent, List<PredicateExp> predicates) : base(node, parent)
        {
            Predicates = predicates;
        }

        public PredicatesDecl(INode? parent, List<PredicateExp> predicates) : base(parent)
        {
            Predicates = predicates;
        }

        public PredicatesDecl(List<PredicateExp> predicates) : base()
        {
            Predicates = predicates;
        }

        public PredicatesDecl(ASTNode node, INode? parent) : base(node, parent)
        {
            Predicates = new List<PredicateExp>();
        }

        public PredicatesDecl(INode? parent) : base(parent)
        {
            Predicates = new List<PredicateExp>();
        }

        public PredicatesDecl() : base()
        {
            Predicates = new List<PredicateExp>();
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var pred in Predicates)
                hash *= pred.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Predicates.GetEnumerator();
        }

        public override PredicatesDecl Copy(INode? newParent = null)
        {
            var newNode = new PredicatesDecl(new ASTNode(Start, End, Line, "", ""), newParent);
            foreach (var node in Predicates)
                newNode.Predicates.Add(node.Copy(newNode));
            newNode.IsHidden = IsHidden;
            return newNode;
        }

        public override void Replace(INode node, INode with)
        {
            for (int i = 0; i < Predicates.Count; i++)
            {
                if (Predicates[i] == node && with is PredicateExp pred)
                    Predicates[i] = pred;
            }
        }

        public override void Add(INode node)
        {
            if (node is PredicateExp exp)
                Predicates.Add(exp);
        }

        public override void Remove(INode node)
        {
            if (node is PredicateExp exp)
                Predicates.Remove(exp);
        }
    }
}
