using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PDDLSharp.Tools;

namespace PDDLSharp.Models.Expressions
{
    public class OrExp : BaseWalkableNode, IExp
    {
        public List<IExp> Options { get; set; }

        public OrExp(ASTNode node, INode? parent, List<IExp> options) : base(node, parent)
        {
            Options = options;
        }

        public OrExp(INode? parent, List<IExp> options) : base(parent)
        {
            Options = options;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var option in Options)
                hash *= option.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Options.GetEnumerator(); ;
        }
    }
}
