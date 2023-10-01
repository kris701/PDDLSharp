using PDDLSharp.Analysers.Visitors;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.Domain;
using PDDLSharp.Models.Problem;
using PDDLSharp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Analysers
{
    public class PDDLAnalyser : IAnalyser
    {
        public IErrorListener Listener { get; internal set; }

        public PDDLAnalyser(IErrorListener listener)
        {
            Listener = listener;
        }

        public void Analyse(PDDLDecl decl)
        {
            AnalyserVisitors visitor = new AnalyserVisitors(Listener, decl);
            visitor.Visit((dynamic)decl.Domain);
            visitor.Visit((dynamic)decl.Problem);
        }
    }
}
