using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Problem
{
    public class SituationDecl : BaseNamedNode, IDecl
    {

        public SituationDecl(ASTNode node, INode? parent, string name) : base(node, parent, name)
        {
        }

        public SituationDecl(INode? parent, string name) : base(parent, name)
        {
        }

        public SituationDecl(string name) : base(name)
        {
        }

        public override bool Equals(object? obj)
        {
            if (obj is SituationDecl other)
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

        public override SituationDecl Copy(INode? newParent = null)
        {
            var newNode = new SituationDecl(new ASTNode(Line, "", ""), newParent, Name);
            newNode.IsHidden = IsHidden;
            return newNode;
        }
    }
}
