using PDDLSharp;
using PDDLSharp.Analysers;
using PDDLSharp.Analysers.PDDL;
using PDDLSharp.CodeGenerators;
using PDDLSharp.CodeGenerators.PDDL;
using PDDLSharp.Contextualisers;
using PDDLSharp.Contextualisers.PDDL;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.PDDL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTools;

namespace PDDLSharp.CodeGenerators.Tests.PDDL
{
    [TestClass]
    public class ComplexPDDLCodeGeneratorTests : BenchmarkBuilder
    {
        public static IEnumerable<object[]> DomainData() => GetDomains();
        public static IEnumerable<object[]> ProblemData() => GetProblems();

        [TestMethod]
        [DynamicData(nameof(DomainData), DynamicDataSourceType.Method)]
        public void Can_Parse_Generate_Parse_Domain(string domain)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<INode> parser = new PDDLParser(listener);
            var initialDecl = parser.ParseAs<DomainDecl>(new FileInfo(domain));
            ICodeGenerator<INode> generator = new PDDLCodeGenerator(listener);

            // ACT
            generator.Generate(initialDecl, "temp.pddl");
            var newDomainDecl = parser.ParseAs<DomainDecl>(new FileInfo("temp.pddl"));

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }

        [TestMethod]
        [DynamicData(nameof(ProblemData), DynamicDataSourceType.Method)]
        public void Can_Parse_Generate_Parse_Problem(string problem)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<INode> parser = new PDDLParser(listener);
            var initialDecl = parser.ParseAs<ProblemDecl>(new FileInfo(problem));
            ICodeGenerator<INode> generator = new PDDLCodeGenerator(listener);

            // ACT
            generator.Generate(initialDecl, "temp.pddl");
            var newDomainDecl = parser.ParseAs<ProblemDecl>(new FileInfo("temp.pddl"));

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }
    }
}
