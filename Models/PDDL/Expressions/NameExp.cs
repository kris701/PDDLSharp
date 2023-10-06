using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Tools;
using PDDLSharp.Models.PDDL;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class NameExp : BaseNamedNode, IExp
    {
        public TypeExp Type { get; set; }

        public NameExp(ASTNode node, INode parent, string name, TypeExp type) : base(node, parent, name)
        {
            Type = type;
        }

        public NameExp(INode parent, string name, TypeExp type) : base(parent, name)
        {
            Type = type;
        }

        public NameExp(string name, TypeExp type) : base(name)
        {
            Type = type;
        }

        public NameExp(ASTNode node, INode parent, string name) : base(node, parent, name)
        {
            Type = new TypeExp(node, this, "");
        }

        public NameExp(INode parent, string name) : base(parent, name)
        {
            Type = new TypeExp(this, "");
        }

        public NameExp(string name) : base(name)
        {
            Type = new TypeExp(this, "");
        }

        public NameExp(NameExp other) : base(other.Name)
        {
            Type = new TypeExp(other.Type);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + Type.GetHashCode();
        }
    }
}
