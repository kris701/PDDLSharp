using PDDLSharp.Models.AST;
using System.Collections;

namespace PDDLSharp.Models.PDDL
{
    public abstract class BaseNamedWalkableNode : BaseNamedNode, IWalkable
    {
        protected BaseNamedWalkableNode(ASTNode node, INode parent, string name) : base(node, parent, name)
        {
        }

        protected BaseNamedWalkableNode(INode parent, string name) : base(parent, name)
        {
        }

        protected BaseNamedWalkableNode(string name) : base(name)
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
    }
}
