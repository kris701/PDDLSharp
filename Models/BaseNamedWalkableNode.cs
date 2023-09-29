using PDDLSharp.Models.AST;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PDDLSharp.Models
{
    public abstract class BaseNamedWalkableNode : BaseNamedNode, IWalkable
    {
        protected BaseNamedWalkableNode(ASTNode node, INode? parent, string name) : base(node, parent, name)
        {
        }

        protected BaseNamedWalkableNode(INode? parent, string name) : base(parent, name)
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public abstract IEnumerator<INode> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
