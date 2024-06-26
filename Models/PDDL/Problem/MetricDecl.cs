﻿using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Models.PDDL.Problem
{
    public class MetricDecl : BaseWalkableNode, IDecl
    {
        public IExp MetricExp { get; set; }
        public string MetricType { get; set; }

        public MetricDecl(ASTNode node, INode? parent, string metricType, IExp metricExp) : base(node, parent)
        {
            MetricType = metricType;
            MetricExp = metricExp;
        }

        public MetricDecl(INode? parent, string metricType, IExp metricExp) : base(parent)
        {
            MetricType = metricType;
            MetricExp = metricExp;
        }

        public MetricDecl(string metricType, IExp metricExp) : base()
        {
            MetricType = metricType;
            MetricExp = metricExp;
        }

        public MetricDecl(ASTNode node, INode? parent, string metricType) : base(node, parent)
        {
            MetricType = metricType;
            MetricExp = new AndExp(this, new List<IExp>());
        }

        public MetricDecl(INode? parent, string metricType) : base(parent)
        {
            MetricType = metricType;
            MetricExp = new AndExp(this, new List<IExp>());
        }

        public MetricDecl(string metricType) : base()
        {
            MetricType = metricType;
            MetricExp = new AndExp(this, new List<IExp>());
        }

        public override bool Equals(object? obj)
        {
            if (obj is MetricDecl other)
            {
                if (!base.Equals(other)) return false;
                if (!MetricType.Equals(other.MetricType)) return false;
                if (!MetricExp.Equals(other.MetricExp)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ MetricType.GetHashCode() ^ MetricExp.GetHashCode();
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return MetricExp;
        }

        public override MetricDecl Copy(INode? newParent = null)
        {
            var newNode = new MetricDecl(new ASTNode(Line, "", ""), newParent, MetricType);
            newNode._metaInfo = new List<System.Reflection.PropertyInfo>(_metaInfo);
            newNode.MetricExp = ((dynamic)MetricExp).Copy(newNode);
            newNode.IsHidden = IsHidden;
            return newNode;
        }

        public override void Replace(INode node, INode with)
        {
            if (MetricExp == node && with is IExp exp)
                MetricExp = exp;
        }
    }
}
