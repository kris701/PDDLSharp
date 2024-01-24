using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Problem
{
    public class ProblemNameDecl : BaseNamedNode, IDecl
    {
        public ProblemNameDecl(ASTNode node, INode? parent, string name) : base(node, parent, name)
        {
        }

        public ProblemNameDecl(INode? parent, string name) : base(parent, name)
        {
        }

        public ProblemNameDecl(string name) : base(name)
        {
        }

        public override bool Equals(object? obj)
        {
            if (obj is ProblemNameDecl other)
            {
                if (!base.Equals(other)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override ProblemNameDecl Copy(INode? newParent = null)
        {
            var newNode = new ProblemNameDecl(new ASTNode(Line, "", ""), newParent, Name);
            newNode.IsHidden = IsHidden;
            return newNode;
        }
    }
}
