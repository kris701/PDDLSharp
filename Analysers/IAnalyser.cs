using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL;

namespace PDDLSharp.Analysers
{
    public interface IAnalyser<T>
    {
        public IErrorListener Listener { get; }

        public void Analyse(T decl);
    }
}
