﻿using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models
{
    public class NoNode : BaseNode, IDecl
    {
        public NoNode() : base(new ASTNode(), null)
        {
        }
    }
}
