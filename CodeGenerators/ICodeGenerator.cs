using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.CodeGenerators
{
    public interface ICodeGenerator<T>
    {
        public IErrorListener Listener { get; }
        public bool Readable { get; set; }

        public string Generate(T node);
        public void Generate(T node, string toFile);
    }
}
