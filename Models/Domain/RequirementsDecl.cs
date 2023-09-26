using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Tools;
using PDDLSharp.Models.Expressions;

namespace PDDLSharp.Models.Domain
{
    public class RequirementsDecl : BaseWalkableNode, IDecl
    {
        public List<NameExp> Requirements {  get; set; }

        public RequirementsDecl(ASTNode node, INode parent, List<NameExp> requirements) : base(node, parent)
        {
            Requirements = requirements;
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
