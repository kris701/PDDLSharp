using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;

namespace PDDLSharp.Analysers
{
    public interface IAnalyser
    {
        public IErrorListener Listener { get; }

        public void Analyse(PDDLDecl decl);
    }
}
