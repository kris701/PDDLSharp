using ErrorListeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Analysers
{
    public interface IAnalyser<T>
    {
        void PreAnalyse(string text, IErrorListener listener);
        void PostAnalyse(T decl, IErrorListener listener);
    }
}
