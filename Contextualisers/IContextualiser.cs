using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Contextualisers
{
    public interface IContextualiser
    {
        public IErrorListener Listener { get; }

        public void Contexturalise(PDDLDecl decl);
    }
}
