﻿using PDDLSharp.Models.AST;
using System.Collections;

namespace PDDLSharp.Models.PDDL
{
    public abstract class BaseListableNode : BaseNode, IListable
    {
        protected BaseListableNode(ASTNode node, INode? parent) : base(node, parent)
        {
        }

        protected BaseListableNode(INode? parent) : base(parent)
        {
        }

        protected BaseListableNode()
        {
        }

        public override bool Equals(object? obj)
        {
            if (obj is BaseListableNode other)
            {
                if (!base.Equals(other)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public abstract IEnumerator<INode> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public abstract void Replace(INode node, INode with);
        public abstract void Add(INode node);
        public void AddRange(List<INode> nodes)
        {
            foreach (var node in nodes)
                Add(node);
        }
        public abstract void Remove(INode node);
        public void RemoveAll(INode node)
        {
            while (Contains(node))
                Remove(node);
        }
        public void RemoveRange(List<INode> nodes)
        {
            foreach (var node in nodes)
                Remove(node);
        }

        public bool Contains(INode node)
        {
            foreach (var subNode in this)
                if (subNode == node)
                    return true;
            return false;
        }
        public bool ContainsAll(List<INode> nodes)
        {
            foreach (var node in nodes)
                if (!Contains(node))
                    return false;
            return true;
        }

        public int Count(INode node)
        {
            int count = 0;
            foreach (var subNode in this)
                if (subNode == node)
                    count++;
            return count;
        }

        public override void RemoveContext()
        {
            base.RemoveContext();
            foreach (var item in this)
                item.RemoveContext();
        }

        public override void RemoveTypes()
        {
            foreach (var item in this)
                item.RemoveTypes();
        }
    }
}
