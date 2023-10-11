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

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override ProblemNameDecl Copy(INode? newParent = null)
        {
            return new ProblemNameDecl(new ASTNode(Start, End, Line, "", ""), newParent, Name);
        }
    }
}
