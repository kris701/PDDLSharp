using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.FastDownward.SAS.Sections
{
    public class MetricDecl : BaseSASNode
    {
        public bool IsUsingMetrics { get; set; }

        public MetricDecl(ASTNode node, bool isUsingMetrics) : base(node)
        {
            IsUsingMetrics = isUsingMetrics;
        }

        public MetricDecl(bool isUsingMetrics)
        {
            IsUsingMetrics = isUsingMetrics;
        }

        public override string? ToString()
        {
            if (IsUsingMetrics)
                return "true";
            return "false";
        }
    }
}
