using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.ASTGenerators
{
    public interface IGenerator
    {
        public IErrorListener Listener { get; }

        public ASTNode Generate(FileInfo file);
        public ASTNode Generate(string text);
    }
}
