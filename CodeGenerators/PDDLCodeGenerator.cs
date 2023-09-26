using PDDL.CodeGenerators.Visitors;
using PDDL.ErrorListeners;
using PDDL.Models;
using PDDL.Models.Domain;
using PDDL.Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDL.CodeGenerators
{
    public class PDDLCodeGenerator : IPDDLCodeGenerator
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
