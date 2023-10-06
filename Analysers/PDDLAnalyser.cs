using PDDLSharp.Analysers.Visitors;
using PDDLSharp.Contextualisers;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
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
            if (!decl.IsContextualised)
            {
                IContextualiser contextualiser = new PDDLContextualiser(Listener);
                contextualiser.Contexturalise(decl);
            }

            AnalyserVisitors visitor = new AnalyserVisitors(Listener, decl);
            visitor.Visit(decl.Domain);
            visitor.Visit(decl.Problem);
        }
    }
}
