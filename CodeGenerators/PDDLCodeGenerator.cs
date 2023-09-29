using PDDLSharp.CodeGenerators.Visitors;
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
    public class PDDLCodeGenerator : ICodeGenerator
    {
        public IErrorListener Listener { get; }

        public PDDLCodeGenerator(IErrorListener listener)
        {
            Listener = listener;
        }

        public void Generate(INode node, string toFile) => File.WriteAllText(toFile, Generate(node));
        public string Generate(INode node)
        {
            GeneratorVisitors visitor = new GeneratorVisitors();
            return visitor.Visit((dynamic)node);
        }
    }
}
