using PDDLSharp.Analysers.Visitors;
using PDDLSharp.Contextualisers;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;

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
