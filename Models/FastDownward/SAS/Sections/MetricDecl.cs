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


        public override bool Equals(object? obj)
        {
            if (obj is MetricDecl other)
            {
                if (IsUsingMetrics != other.IsUsingMetrics) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return IsUsingMetrics.GetHashCode();
        }
    }
}
