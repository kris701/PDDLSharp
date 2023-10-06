using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.Domain;
using PDDLSharp.Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Parsers
{
    public interface IParser<T>
    {
        public IErrorListener Listener { get; }

        public T Parse(string file);
        public U ParseAs<U>(string file) where U : T;
    }
}
