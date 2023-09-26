using PDDL.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PDDL.Tools;

namespace PDDL.Models.Problem
{
    public class GoalDecl : BaseWalkableNode, IDecl
    {
        public IExp GoalExp { get; set; }

        public GoalDecl(ASTNode node, INode parent, IExp goalExp) : base(node, parent)
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
    }
}
