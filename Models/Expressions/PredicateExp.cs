﻿using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Tools;

namespace PDDLSharp.Models.Expressions
{
    public class PredicateExp : BaseWalkableNode, IExp, INamedNode
    {
        public string Name { get; set; }
        public List<NameExp> Arguments { get; set; }

        public PredicateExp(ASTNode node, INode parent, string name, List<NameExp> arguments) : base(node, parent)
        {
            Name = name;
            Arguments = arguments;
        }

        public override int GetHashCode()
        {
            int hash = Name.GetHashCode() + base.GetHashCode();
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