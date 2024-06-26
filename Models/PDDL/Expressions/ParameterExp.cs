﻿using PDDLSharp.Models.AST;
using PDDLSharp.Tools;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class ParameterExp : BaseListableNode
    {
        public List<NameExp> Values { get; set; }

        public ParameterExp(ASTNode node, INode? parent, List<NameExp> values) : base(node, parent)
        {
            Values = values;
        }

        public ParameterExp(INode? parent, List<NameExp> values) : base(parent)
        {
            Values = values;
        }

        public ParameterExp(List<NameExp> values) : base()
        {
            Values = values;
        }

        public ParameterExp(ASTNode node, INode? parent) : base(node, parent)
        {
            Values = new List<NameExp>();
        }

        public ParameterExp(INode? parent) : base(parent)
        {
            Values = new List<NameExp>();
        }

        public ParameterExp() : base()
        {
            Values = new List<NameExp>();
        }

        public override bool Equals(object? obj)
        {
            if (obj is ParameterExp other)
            {
                if (!base.Equals(other)) return false;
                if (!EqualityHelper.AreListsEqualUnordered(Values, other.Values)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var param in Values)
                hash *= param.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            var retStr = "";
            foreach (var value in Values)
                retStr += $" {value.Name}";
            return retStr;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        public override ParameterExp Copy(INode? newParent = null)
        {
            var newNode = new ParameterExp(new ASTNode(Line, "", ""), newParent);
            newNode._metaInfo = new List<System.Reflection.PropertyInfo>(_metaInfo);
            foreach (var node in Values)
                newNode.Values.Add(((dynamic)node).Copy(newNode));
            newNode.IsHidden = IsHidden;
            return newNode;
        }

        public override void Replace(INode node, INode with)
        {
            for (int i = 0; i < Values.Count; i++)
            {
                if (Values[i] == node && with is NameExp name)
                    Values[i] = name;
            }
        }

        public override void Add(INode node)
        {
            if (node is NameExp exp)
                Values.Add(exp);
        }

        public override void Remove(INode node)
        {
            if (node is NameExp exp)
                Values.Remove(exp);
        }

        public override void RemoveTypes()
        {

        }
    }
}
