﻿using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Models.PDDL.Problem
{
    public class GoalDecl : BaseWalkableNode, IDecl
    {
        public IExp GoalExp { get; set; }

        public GoalDecl(ASTNode node, INode? parent, IExp goalExp) : base(node, parent)
        {
            GoalExp = goalExp;
        }

        public GoalDecl(INode? parent, IExp goalExp) : base(parent)
        {
            GoalExp = goalExp;
        }

        public GoalDecl(IExp goalExp) : base()
        {
            GoalExp = goalExp;
        }

        public GoalDecl(ASTNode node, INode? parent) : base(node, parent)
        {
            GoalExp = new AndExp(this, new List<IExp>());
        }

        public GoalDecl(INode? parent) : base(parent)
        {
            GoalExp = new AndExp(this, new List<IExp>());
        }

        public GoalDecl() : base()
        {
            GoalExp = new AndExp(this, new List<IExp>());
        }

        public override bool Equals(object? obj)
        {
            if (obj is GoalDecl other)
            {
                if (!base.Equals(other)) return false;
                if (!GoalExp.Equals(other.GoalExp)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ GoalExp.GetHashCode();
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return GoalExp;
        }

        public override GoalDecl Copy(INode? newParent = null)
        {
            var newNode = new GoalDecl(new ASTNode(Line, "", ""), newParent);
            newNode._metaInfo = new List<System.Reflection.PropertyInfo>(_metaInfo);
            newNode.GoalExp = ((dynamic)GoalExp).Copy(newNode);
            newNode.IsHidden = IsHidden;
            return newNode;
        }

        public override void Replace(INode node, INode with)
        {
            if (GoalExp == node && with is IExp exp)
                GoalExp = exp;
        }
    }
}
