﻿using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class ParameterExp : BaseWalkableNode
    {
        public List<NameExp> Values { get; set; }

        public ParameterExp(ASTNode node, INode parent, List<NameExp> values) : base(node, parent)
        {
            Values = values;
        }

        public ParameterExp(INode parent, List<NameExp> values) : base(parent)
        {
            Values = values;
        }

        public ParameterExp(List<NameExp> values) : base()
        {
            Values = values;
        }

        public ParameterExp() : base()
        {
            Values = new List<NameExp>();
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
