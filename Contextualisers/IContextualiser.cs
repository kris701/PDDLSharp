using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;

namespace PDDLSharp.Contextualisers
{
    public interface IContextualiser
    {
        public IErrorListener Listener { get; }

        public void Contexturalise(PDDLDecl decl);
    }
}
