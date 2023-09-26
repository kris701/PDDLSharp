using PDDL.Models.AST;
using PDDL.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDL.Models.Problem
{
    public class DomainNameRefDecl : BaseNode, IDecl, INamedNode
    {
        public string Name { get; set; }

        public DomainNameRefDecl(ASTNode node, INode parent, string name) : base(node, parent)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"(:domain {Name})";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + Name.GetHashCode();
        }
    }
}
