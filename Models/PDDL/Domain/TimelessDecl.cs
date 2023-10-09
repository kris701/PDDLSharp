﻿using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class TimelessDecl : BaseWalkableNode<TimelessDecl>, IDecl
    {
        public List<PredicateExp> Items { get; set; }

        public TimelessDecl(ASTNode node, INode parent, List<PredicateExp> timeless) : base(node, parent)
        {
            Items = timeless;
        }

        public TimelessDecl(INode parent, List<PredicateExp> timeless) : base(parent)
        {
            Items = timeless;
        }

        public TimelessDecl(List<PredicateExp> timeless) : base()
        {
            Items = timeless;
        }

        public TimelessDecl() : base()
        {
            Items = new List<PredicateExp>();
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var item in Items)
                hash *= item.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public override TimelessDecl Copy(INode newParent)
        {
            var newNode = new TimelessDecl(new ASTNode(Start, End, Line, "", ""), newParent, new List<PredicateExp>());
            foreach (var node in Items)
                newNode.Items.Add(node.Copy(newNode));
            return newNode;
        }
    }
}
