using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class RequirementsDecl : BaseWalkableNode<RequirementsDecl>, IDecl
    {
        public List<NameExp> Requirements { get; set; }

        public RequirementsDecl(ASTNode node, INode parent, List<NameExp> requirements) : base(node, parent)
        {
            Requirements = requirements;
        }

        public RequirementsDecl(INode parent, List<NameExp> requirements) : base(parent)
        {
            Requirements = requirements;
        }

        public RequirementsDecl(List<NameExp> requirements) : base()
        {
            Requirements = requirements;
        }

        public RequirementsDecl() : base()
        {
            Requirements = new List<NameExp>();
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var req in Requirements)
                hash *= req.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Requirements.GetEnumerator();
        }

        public override RequirementsDecl Copy(INode newParent)
        {
            var newNode = new RequirementsDecl(new ASTNode(Start, End, Line, "", ""), newParent, new List<NameExp>());
            foreach (var node in Requirements)
                newNode.Requirements.Add(node.Copy(newNode));
            return newNode;
        }
    }
}
