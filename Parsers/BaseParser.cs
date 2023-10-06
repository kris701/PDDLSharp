using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.Plans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Parsers
{
    public abstract class BaseParser<T> : IParser<T>
    {
        public IErrorListener Listener { get; }

        public BaseParser(IErrorListener listener)
        {
            Listener = listener;
        }

        public abstract T Parse(string file);
        public virtual U ParseAs<U>(string file) where U : T
        {
            return (U)Parse(file);
        }
    }
}
