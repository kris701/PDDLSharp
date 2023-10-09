using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Problem
{
    public class SituationDecl : BaseNamedNode, IDecl
    {

        public SituationDecl(ASTNode node, INode parent, string name) : base(node, parent, name)
        {
        }

        public SituationDecl(INode parent, string name) : base(parent, name)
        {
        }

        public SituationDecl(string name) : base(name)
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override SituationDecl Copy(INode newParent)
        {
            return new SituationDecl(new ASTNode(Start, End, Line, "", ""), newParent, Name);
        }
    }
}
