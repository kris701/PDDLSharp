﻿using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class FunctionsDecl : BaseWalkableNode<FunctionsDecl>, IDecl
    {
        public List<PredicateExp> Functions { get; set; }

        public FunctionsDecl(ASTNode node, INode parent, List<PredicateExp> functions) : base(node, parent)
        {
            Functions = functions;
        }

        public FunctionsDecl(INode parent, List<PredicateExp> functions) : base(parent)
        {
            Functions = functions;
        }

        public FunctionsDecl(List<PredicateExp> functions) : base()
        {
            Functions = functions;
        }

        public FunctionsDecl() : base()
        {
            Functions = new List<PredicateExp>();
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var pred in Functions)
                hash *= pred.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Functions.GetEnumerator();
        }

        public override FunctionsDecl Copy(INode newParent)
        {
            var newNode = new FunctionsDecl(new ASTNode(Start, End, Line, "", ""), newParent, new List<PredicateExp>());
            foreach (var node in Functions)
                newNode.Functions.Add(node.Copy(newNode));
            return newNode;
        }
    }
}
