using PDDLSharp.Models.AST;
using System.Collections;

namespace PDDLSharp.Models.PDDL
{
    public abstract class BaseNamedWalkableNode : BaseNamedNode, IWalkable
    {
        protected BaseNamedWalkableNode(ASTNode node, INode? parent, string name) : base(node, parent, name)
        {
        }

        protected BaseNamedWalkableNode(INode? parent, string name) : base(parent, name)
        {
        }

        protected BaseNamedWalkableNode(string name) : base(name)
        {
        }

        public override bool Equals(object? obj)
        {
            if (obj is BaseNamedWalkableNode other)
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
