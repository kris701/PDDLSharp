using PDDL.Models.AST;
using PDDL.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDL.Tools;

namespace PDDL.Models.Expressions
{
    public class NameExp : BaseNode, IExp, INamedNode
    {
        public string Name { get; set; }
        public TypeExp? Type { get; set; }

        public NameExp(ASTNode node, INode? parent, string name, TypeExp? type) : base(node, parent)
        {
            Name = name;
            Type = type;
        }

        public NameExp(ASTNode node, INode? parent, string name) : base(node, parent)
        {
            Name = name;
            Type = new TypeExp(node, this, "");
        }

        public override string ToString()
        {
            if (Type == null || Type.Name == "")
                return $"({Name})";
            else
                return $"({Name} - {Type})";
        }

        public override int GetHashCode()
        {
            if (Type != null)
                return Name.GetHashCode() + base.GetHashCode() + Type.GetHashCode();
            return Name.GetHashCode() + base.GetHashCode();
        }
    }
}
