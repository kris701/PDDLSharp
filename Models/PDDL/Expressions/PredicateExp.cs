﻿using PDDLSharp.Models.AST;
using PDDLSharp.Tools;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class PredicateExp : BaseNamedListableNode, IExp
    {
        public List<NameExp> Arguments { get; set; }

        public PredicateExp(ASTNode node, INode? parent, string name, List<NameExp> arguments) : base(node, parent, name)
        {
            Arguments = arguments;
        }

        public PredicateExp(INode? parent, string name, List<NameExp> arguments) : base(parent, name)
        {
            Arguments = arguments;
        }

        public PredicateExp(string name, List<NameExp> arguments) : base(name)
        {
            Arguments = arguments;
        }

        public PredicateExp(ASTNode node, INode? parent, string name) : base(node, parent, name)
        {
            Arguments = new List<NameExp>();
        }

        public PredicateExp(INode? parent, string name) : base(parent, name)
        {
            Arguments = new List<NameExp>();
        }

        public PredicateExp(string name) : base(name)
        {
            Arguments = new List<NameExp>();
        }


        public override bool Equals(object? obj)
        {
            if (obj is PredicateExp other)
            {
                if (!base.Equals(other)) return false;
                if (!EqualityHelper.AreListsEqual(Arguments, other.Arguments)) return false;
                return true;
            }
            return false;
        }

        // The order is important!
        // Based on: https://stackoverflow.com/a/30758270
        public override int GetHashCode()
        {
            const int seed = 487;
            const int modifier = 31;
            unchecked
            {
                return base.GetHashCode() + Arguments.Aggregate(seed, (current, item) =>
                    (current * modifier) + item.GetHashCode());
            }
        }

        public override string? ToString()
        {
            var retStr = Name;
            foreach (var arg in Arguments)
                retStr += $" {arg}";
            return retStr;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Arguments.GetEnumerator();
        }

        public override PredicateExp Copy(INode? newParent = null)
        {
            var newNode = new PredicateExp(new ASTNode(Line, "", ""), newParent, Name);
            newNode._metaInfo = new List<System.Reflection.PropertyInfo>(_metaInfo);
            foreach (var node in Arguments)
                newNode.Arguments.Add(((dynamic)node).Copy(newNode));
            newNode.IsHidden = IsHidden;
            return newNode;
        }

        public override void Replace(INode node, INode with)
        {
            for (int i = 0; i < Arguments.Count; i++)
            {
                if (Arguments[i] == node && with is NameExp name)
                    Arguments[i] = name;
            }
        }

        public override void Add(INode node)
        {
            if (node is NameExp exp)
                Arguments.Add(exp);
        }

        public override void Remove(INode node)
        {
            if (node is NameExp exp)
                Arguments.Remove(exp);
        }
    }
}
