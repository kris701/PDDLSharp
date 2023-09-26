﻿using PDDL.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDL.Tools;

namespace PDDL.Models.Expressions
{
    public class NotExp : BaseWalkableNode, IExp
    {
        public IExp Child { get; set; }

        public NotExp(ASTNode node, INode parent, IExp child) : base(node, parent)
        {
            Child = child;
        }

        public override string ToString()
        {
            return $"(not {Child})";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() * Child.GetHashCode();
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Child;
        }
    }
}
