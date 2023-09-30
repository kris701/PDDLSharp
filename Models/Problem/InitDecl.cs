﻿using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Tools;

namespace PDDLSharp.Models.Problem
{
    public class InitDecl : BaseWalkableNode, IDecl
    {
        public List<IExp> Predicates { get; set; }

        public InitDecl(ASTNode node, INode? parent, List<IExp> predicates) : base(node, parent)
        {
            Predicates = predicates;
        }

        public InitDecl(INode? parent, List<IExp> predicates) : base(parent)
        {
            Predicates = predicates;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach(var pred in Predicates)
                hash *= pred.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Predicates.GetEnumerator();
        }
    }
}
