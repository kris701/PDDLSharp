using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Problem
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

        public MetricDecl(INode parent, string metricType, IExp metricExp) : base(parent)
        {
            MetricType = metricType;
            MetricExp = metricExp;
        }

        public MetricDecl(string metricType, IExp metricExp) : base()
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
