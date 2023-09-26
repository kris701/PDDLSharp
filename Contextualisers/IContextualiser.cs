using ErrorListeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contextualisers
{
    public interface IContextualiser<T>
    {
        void Contexturalise(T decl, IErrorListener listener);
    }
}
