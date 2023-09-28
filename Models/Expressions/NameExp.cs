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
    public class NameExp : BaseNode, IExp, INamedNode
    {
        public string Name { get; set; }
        public TypeExp Type { get; set; }

        public NameExp(ASTNode node, INode? parent, string name, TypeExp type) : base(node, parent)
        {
            Name = name;
            Type = type;
        }

        public NameExp(ASTNode node, INode? parent, string name) : base(node, parent)
        {
            Name = name;
            Type = new TypeExp(node, this, "");
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() + base.GetHashCode() + Type.GetHashCode();
        }
    }
}