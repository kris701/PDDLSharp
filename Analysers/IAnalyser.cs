using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL;

namespace PDDLSharp.Analysers
{
    public interface IAnalyser
    {
        public IErrorListener Listener { get; }

        public void Analyse(PDDLDecl decl);
    }
}
