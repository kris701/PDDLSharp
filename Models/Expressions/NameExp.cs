﻿using PDDLSharp.Models.AST;
using PDDLSharp.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Tools;

namespace PDDLSharp.Models.Expressions
{
    public class NameExp : BaseNamedNode, IExp
    {
        public TypeExp Type { get; set; }

        public NameExp(ASTNode node, INode? parent, string name, TypeExp type) : base(node, parent, name)
        {
            Type = type;
        }

        public NameExp(ASTNode node, INode? parent, string name) : base(node, parent, name)
        {
            Type = new TypeExp(node, this, "");
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + Type.GetHashCode();
        }
    }
}
