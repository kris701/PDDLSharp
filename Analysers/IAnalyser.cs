using PDDL.ErrorListeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDL.Analysers
{
    public interface IAnalyser<T>
    {
        IErrorListener Listener { get; }

        void PreAnalyse(string text);
        void PostAnalyse(T decl);
    }
}
