﻿using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.Domain
{
    public class DomainNameDecl : BaseNode, IDecl, INamedNode
    {
        public string Name { get; set; }

        public DomainNameDecl(ASTNode node, INode? parent, string name) : base(node, parent)
        {
            Name = name;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + Name.GetHashCode();
        }
    }
}
