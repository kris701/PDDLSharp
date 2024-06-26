﻿using PDDLSharp.Models.AST;
using PDDLSharp.Tools;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class AndExp : BaseListableNode, IExp
    {
        public List<IExp> Children { get; set; }

        public AndExp(ASTNode node, INode? parent, List<IExp> children) : base(node, parent)
        {
            Children = children;
        }

        public AndExp(INode? parent, List<IExp> children) : base(parent)
        {
            Children = children;
        }

        public AndExp(List<IExp> children) : base()
        {
            Children = children;
        }

        public AndExp(ASTNode node, INode? parent) : base(node, parent)
        {
            Children = new List<IExp>();
        }

        public AndExp(INode? parent) : base(parent)
        {
            Children = new List<IExp>();
        }

        public AndExp() : base()
        {
            Children = new List<IExp>();
        }

        public override bool Equals(object? obj)
        {
            if (obj is AndExp other)
            {
                if (!base.Equals(other)) return false;
                if (!EqualityHelper.AreListsEqualUnordered(Children, other.Children)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var child in Children)
                hash *= child.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        public override AndExp Copy(INode? newParent = null)
        {
            var newNode = new AndExp(new ASTNode(Line, "", ""), newParent);
            newNode._metaInfo = new List<System.Reflection.PropertyInfo>(_metaInfo);
            foreach (var node in Children)
                newNode.Children.Add(((dynamic)node).Copy(newNode));
            newNode.IsHidden = IsHidden;
            return newNode;
        }

        public override void Replace(INode node, INode with)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i] == node && with is IExp asExp)
                    Children[i] = asExp;
            }
        }

        public override void Add(INode node)
        {
            if (node is IExp exp)
                Children.Add(exp);
        }

        public override void Remove(INode node)
        {
            if (node is IExp exp)
                Children.Remove(exp);
        }
    }
}
