﻿using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Tools;

namespace PDDLSharp.Models.Expressions
{
    public class PredicateExp : BaseNamedWalkableNode, IExp
    {
        public List<NameExp> Arguments { get; set; }

        public PredicateExp(ASTNode node, INode parent, string name, List<NameExp> arguments) : base(node, parent, name)
        {
            Arguments = arguments;
        }

        public PredicateExp(INode parent, string name, List<NameExp> arguments) : base(parent, name)
        {
            Arguments = arguments;
        }

        public PredicateExp(string name, List<NameExp> arguments) : base(name)
        {
            Arguments = arguments;
        }

        public PredicateExp(string name) : base(name)
        {
            Arguments = new List<NameExp>();
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            foreach (var arg in Arguments)
                hash *= arg.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Arguments.GetEnumerator();
        }
    }
}
