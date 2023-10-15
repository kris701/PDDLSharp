using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.SAS.Sections
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
