using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Analysers
{
    public interface IAnalyser
    {
        public IErrorListener Listener { get; }

        public void Analyse(PDDLDecl decl);
    }
}
