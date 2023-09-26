using PDDL.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PDDL.Tools;
using PDDL.Models.Expressions;

namespace PDDL.Models.Domain
{
    public class ExtendsDecl : BaseWalkableNode, IDecl
    {
        public List<NameExp> Extends { get; set; }

        public ExtendsDecl(ASTNode node, INode parent, List<NameExp> extends) : base(node, parent)
        {
            Extends = extends;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach(var extend in Extends)
                hash *= extend.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Extends.GetEnumerator();
        }
    }
}
