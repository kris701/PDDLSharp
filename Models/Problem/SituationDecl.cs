﻿using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.Problem
{
    public class SituationDecl : BaseNamedNode, IDecl
    {

        public SituationDecl(ASTNode node, INode? parent, string name) : base(node, parent, name)
        {
        }

        public SituationDecl(INode? parent, string name) : base(parent, name)
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
