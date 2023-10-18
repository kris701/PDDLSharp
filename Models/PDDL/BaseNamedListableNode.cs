using PDDLSharp.Models.AST;
using System.Collections;

namespace PDDLSharp.Models.PDDL
{
    public abstract class BaseNamedListableNode : BaseNamedNode, IListable
    {
        protected BaseNamedListableNode(ASTNode node, INode? parent, string name) : base(node, parent, name)
        {
        }

        protected BaseNamedListableNode(INode? parent, string name) : base(parent, name)
        {
        }

        protected BaseNamedListableNode(string name) : base(name)
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

        public virtual void Add(INode node)
        {
            // Default does nothing
        }

        public virtual void Remove(INode node)
        {
            // Default does nothing
        }

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
