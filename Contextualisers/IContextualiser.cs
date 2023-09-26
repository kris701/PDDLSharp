using PDDL.ErrorListeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDL.Contextualisers
{
    public interface IContextualiser<T>
    {
        IErrorListener Listener { get; }

        void Contexturalise(T decl);
    }
}
