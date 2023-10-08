using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class PredicatesDecl : BaseWalkableNode, IDecl
    {
        public List<PredicateExp> Predicates { get; set; }

        public PredicatesDecl(ASTNode node, INode parent, List<PredicateExp> predicates) : base(node, parent)
        {
            Predicates = predicates;
        }

        public PredicatesDecl(INode parent, List<PredicateExp> predicates) : base(parent)
        {
            Predicates = predicates;
        }

        public PredicatesDecl(List<PredicateExp> predicates) : base()
        {
            Predicates = predicates;
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
    }
}
