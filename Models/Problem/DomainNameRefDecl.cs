﻿using PDDLSharp.Models.AST;
using PDDLSharp.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.Problem
{
    public class DomainNameRefDecl : BaseNamedNode, IDecl
    {

        public DomainNameRefDecl(ASTNode node, INode? parent, string name) : base(node, parent, name)
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
