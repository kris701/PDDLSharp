﻿using PDDLSharp.ErrorListeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Parsers.Visitors
{
    public interface IVisitor<AST, ParentT, OutT>
    {
        OutT Visit(AST node, ParentT parent, IErrorListener listener);
    }
}
