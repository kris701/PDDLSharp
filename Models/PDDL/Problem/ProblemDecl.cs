﻿using PDDLSharp.Models.AST;

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

        public override ProblemDecl Copy(INode? newParent = null)
        {
            var newNode = new ProblemDecl(new ASTNode(Start, End, Line, "", ""));

            if (Name != null)
                newNode.Name = Name.Copy(newNode);
            if (DomainName != null)
                newNode.DomainName = DomainName.Copy(newNode);
            if (Situation != null)
                newNode.Situation = Situation.Copy(newNode);
            if (Objects != null)
                newNode.Objects = Objects.Copy(newNode);
            if (Init != null)
                newNode.Init = Init.Copy(newNode);
            if (Goal != null)
                newNode.Goal = Goal.Copy(newNode);
            if (Metric != null)
                newNode.Metric = Metric.Copy(newNode);

            return newNode;
        }
    }
}
