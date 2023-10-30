using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.FastDownward.SAS.Sections
{
    public class GoalStateDecl : BaseSASNode
    {
        public List<ValuePair> Goals { get; set; }

        public GoalStateDecl(ASTNode node, List<ValuePair> goals) : base(node)
        {
            Goals = goals;
        }

        public GoalStateDecl(List<ValuePair> goals)
        {
            Goals = goals;
        }

        public override string? ToString()
        {
            var retStr = "";
            foreach (var value in Goals)
                retStr += $"{value}, ";
            return retStr.Trim();
        }
    }
}
