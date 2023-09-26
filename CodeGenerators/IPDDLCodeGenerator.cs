using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.Domain;
using PDDLSharp.Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.CodeGenerators
{
    public interface IPDDLCodeGenerator
    {
        public IErrorListener Listener { get; }

        public string Generate(INode node);
        public void Generate(INode node, string toFile);
    }
}
