using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PDDLSharp.Tools;
using PDDLSharp.Models.PDDL;

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
    }
}
