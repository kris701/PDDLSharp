using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Problem
{
    public class GoalDecl : BaseWalkableNode, IDecl
    {
        public IExp GoalExp { get; set; }

        public GoalDecl(ASTNode node, INode parent, IExp goalExp) : base(node, parent)
        {
            GoalExp = goalExp;
        }

        public GoalDecl(INode parent, IExp goalExp) : base(parent)
        {
            GoalExp = goalExp;
        }

        public GoalDecl(IExp goalExp) : base()
        {
            GoalExp = goalExp;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + GoalExp.GetHashCode();
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return GoalExp;
        }

        public override GoalDecl Copy(INode newParent)
        {
            var newNode = new GoalDecl(new ASTNode(Start, End, Line, "", ""), newParent, null);
            newNode.GoalExp = ((dynamic)GoalExp).Copy(newNode);
            return newNode;
        }
    }
}
