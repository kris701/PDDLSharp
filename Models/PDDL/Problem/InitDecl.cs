using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Problem
{
    public class InitDecl : BaseWalkableNode, IDecl
    {
        public List<IExp> Predicates { get; set; }

        public InitDecl(ASTNode node, INode? parent, List<IExp> predicates) : base(node, parent)
        {
            Predicates = predicates;
        }

        public InitDecl(INode? parent, List<IExp> predicates) : base(parent)
        {
            Predicates = predicates;
        }

        public InitDecl(List<IExp> predicates) : base()
        {
            Predicates = predicates;
        }

        public InitDecl(ASTNode node, INode? parent) : base(node, parent)
        {
            Predicates = new List<IExp>();
        }

        public InitDecl(INode? parent) : base(parent)
        {
            Predicates = new List<IExp>();
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

        public override InitDecl Copy(INode? newParent = null)
        {
            var newNode = new InitDecl(new ASTNode(Start, End, Line, "", ""), newParent);
            foreach (var node in Predicates)
                newNode.Predicates.Add(((dynamic)node).Copy(newNode));
            return newNode;
        }

        public override void Replace(INode node, INode with)
        {
            for(var i = 0; i < Predicates.Count; i++)
            {
                if (Predicates[i] == node && with is IExp exp)
                    Predicates[i] = exp;
            }
        }
    }
}
