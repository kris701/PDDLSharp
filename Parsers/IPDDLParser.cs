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
    public interface IPDDLParser
    {
        public IErrorListener Listener { get; }

        public PDDLDecl Parse(string domainFile, string problemFile);
        public T? ParseAs<T>(string file) where T : INode;
    }
}
