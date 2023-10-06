using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PDDLSharp.Tools;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class ExtendsDecl : BaseWalkableNode, IDecl
    {
        public List<NameExp> Extends { get; set; }

        public ExtendsDecl(ASTNode node, INode parent, List<NameExp> extends) : base(node, parent)
        {
            Extends = extends;
        }

        public ExtendsDecl(INode parent, List<NameExp> extends) : base(parent)
        {
            Extends = extends;
        }

        public ExtendsDecl(List<NameExp> extends) : base()
        {
            Extends = extends;
        }

        public ExtendsDecl() : base()
        {
            Extends = new List<NameExp>();
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var extend in Extends)
                hash *= extend.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Extends.GetEnumerator();
        }
    }
}
