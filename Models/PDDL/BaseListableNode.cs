using PDDLSharp.Models.AST;
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
        public abstract void Remove(INode node);

        public bool Contains(INode node)
        {
            foreach (var subNode in this)
                if (subNode == node)
                    return true;
            return false;
        }

        public int Count(INode node)
        {
            int count = 0;
            foreach (var subNode in this)
                if (subNode == node)
                    count++;
            return count;
        }
    }
}
