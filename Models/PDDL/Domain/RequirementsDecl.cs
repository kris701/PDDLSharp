using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class RequirementsDecl : BaseWalkableNode, IDecl
    {
        public List<NameExp> Requirements { get; set; }

        public RequirementsDecl(ASTNode node, INode? parent, List<NameExp> requirements) : base(node, parent)
        {
            Requirements = requirements;
        }

        public RequirementsDecl(INode? parent, List<NameExp> requirements) : base(parent)
        {
            Requirements = requirements;
        }

        public RequirementsDecl(List<NameExp> requirements) : base()
        {
            Requirements = requirements;
        }

        public RequirementsDecl(ASTNode node, INode? parent) : base(node, parent)
        {
            Requirements = new List<NameExp>();
        }

        public RequirementsDecl(INode? parent) : base(parent)
        {
            Requirements = new List<NameExp>();
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

        public override RequirementsDecl Copy(INode? newParent = null)
        {
            var newNode = new RequirementsDecl(new ASTNode(Start, End, Line, "", ""), newParent);
            foreach (var node in Requirements)
                newNode.Requirements.Add(node.Copy(newNode));
            return newNode;
        }

        public override void Replace(INode node, INode with)
        {
            for (int i = 0; i < Requirements.Count; i++)
            {
                if (Requirements[i] == node && with is NameExp name)
                    Requirements[i] = name;
            }
        }
    }
}
