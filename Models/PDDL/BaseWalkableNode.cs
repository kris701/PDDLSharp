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
