﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.ASTGenerators
{
    public interface IASTParser<T>
    {
        T Parse(string text);
        string TokenizeSpecials(string text);
    }
}
