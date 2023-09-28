using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models
{
    public abstract class BaseNamedNode : BaseNode, INamedNode
    {
        public string Name { get; set; }

        protected BaseNamedNode(ASTNode node, INode? parent, string name) : base(node, parent)
        {
            Name = name;
        }

        public override string? ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() * Name.GetHashCode();
        }
    }
}
