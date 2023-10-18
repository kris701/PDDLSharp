using PDDLSharp.Models.AST;
using System.Collections;

namespace PDDLSharp.Models.PDDL
{
    public abstract class BaseWalkableNode : BaseNode, IWalkable
    {
        protected BaseWalkableNode(ASTNode node, INode? parent) : base(node, parent)
        {
        }

        protected BaseWalkableNode(INode? parent) : base(parent)
        {
        }

        protected BaseWalkableNode()
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
            throw new NotImplementedException();
        }

        public virtual void Remove(INode node)
        {
            throw new NotImplementedException();
        }
    }
}
