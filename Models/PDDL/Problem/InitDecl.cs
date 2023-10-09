using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Problem
{
    public class InitDecl : BaseWalkableNode<InitDecl>, IDecl
    {
        public List<IExp> Predicates { get; set; }

        public InitDecl(ASTNode node, INode parent, List<IExp> predicates) : base(node, parent)
        {
            Predicates = predicates;
        }

        public InitDecl(INode parent, List<IExp> predicates) : base(parent)
        {
            Predicates = predicates;
        }

        public InitDecl(List<IExp> predicates) : base()
        {
            Predicates = predicates;
        }

        public InitDecl() : base()
        {
            Predicates = new List<IExp>();
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

        public override InitDecl Copy(INode newParent)
        {
            var newNode = new InitDecl(new ASTNode(Start, End, Line, "", ""), newParent, new List<IExp>());
            foreach (var node in Predicates)
                newNode.Predicates.Add(((dynamic)node).Copy(newNode));
            return newNode;
        }
    }
}
