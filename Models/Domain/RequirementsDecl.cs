using PDDL.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDL.Tools;
using PDDL.Models.Expressions;

namespace PDDL.Models.Domain
{
    public class RequirementsDecl : BaseWalkableNode, IDecl
    {
        public List<NameExp> Requirements {  get; set; }

        public RequirementsDecl(ASTNode node, INode parent, List<NameExp> requirements) : base(node, parent)
        {
            Requirements = requirements;
        }

        public override string ToString()
        {
            var reqStr = "";
            foreach (var requirement in Requirements)
                reqStr += $" {requirement}";
            return $"(:requirements{reqStr})";
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var req in Requirements)
                hash *= req.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Requirements.GetEnumerator();
        }
    }
}
