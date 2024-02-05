using PDDLSharp.Models.AST;
using PDDLSharp.Tools;

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

        public override bool Equals(object? obj)
        {
            if (obj is GoalStateDecl other)
            {
                if (!EqualityHelper.AreListsEqual(Goals, other.Goals)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hash = 1;
            foreach (var child in Goals)
                hash ^= child.GetHashCode();
            return hash;
        }

        public GoalStateDecl Copy()
        {
            var goals = new List<ValuePair>();
            foreach (var goal in Goals)
                goals.Add(goal.Copy());
            return new GoalStateDecl(goals);
        }
    }
}
