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
    public abstract class BaseWalkableNode : BaseNode, IWalkable
    {
        protected BaseWalkableNode(ASTNode node, INode? parent) : base(node, parent)
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
