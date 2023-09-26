using PDDL.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDL.Tools;

namespace PDDL.Models.Problem
{
    public class MetricDecl : BaseWalkableNode, IDecl
    {
        public IExp MetricExp { get; set; }
        public string MetricType { get; set; }

        public MetricDecl(ASTNode node, INode parent, string metricType, IExp metricExp) : base(node, parent)
        {
            MetricType = metricType;
            MetricExp = metricExp;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + MetricType.GetHashCode() + MetricExp.GetHashCode();
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return MetricExp;
        }
    }
}
