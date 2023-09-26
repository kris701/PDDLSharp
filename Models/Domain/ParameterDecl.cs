using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PDDLSharp.Tools;
using PDDLSharp.Models.Expressions;

namespace PDDLSharp.Models.Domain
{
    public class ParameterDecl : BaseWalkableNode
    {
        public List<NameExp> Values { get; set; }

        public ParameterDecl(ASTNode node, INode parent, List<NameExp> values) : base(node, parent)
        {
            Values = values;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var param in Values)
                hash *= param.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Values.GetEnumerator();
        }
    }
}
