using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Problem
{
    public class ProblemDecl : BaseWalkableNode, IDecl
    {
        public ProblemNameDecl? Name { get; set; }
        public DomainNameRefDecl? DomainName { get; set; }
        public SituationDecl? Situation { get; set; }
        public ObjectsDecl? Objects { get; set; }
        public InitDecl? Init { get; set; }
        public GoalDecl? Goal { get; set; }
        public MetricDecl? Metric { get; set; }

        public ProblemDecl(ASTNode node) : base(node, null) { }
        public ProblemDecl() : base(null) { }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            if (Name != null)
                hash *= Name.GetHashCode();
            if (DomainName != null)
                hash *= DomainName.GetHashCode();
            if (Situation != null)
                hash *= Situation.GetHashCode();
            if (Objects != null)
                hash *= Objects.GetHashCode();
            if (Init != null)
                hash *= Init.GetHashCode();
            if (Goal != null)
                hash *= Goal.GetHashCode();
            if (Metric != null)
                hash *= Metric.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            if (Name != null)
                yield return Name;
            if (DomainName != null)
                yield return DomainName;
            if (Situation != null)
                yield return Situation;
            if (Objects != null)
                yield return Objects;
            if (Init != null)
                yield return Init;
            if (Goal != null)
                yield return Goal;
            if (Metric != null)
                yield return Metric;
        }
    }
}
