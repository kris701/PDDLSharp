using PDDL.Analysers;
using PDDL.CodeGenerators;
using PDDL.Contextualisers;
using PDDL.ErrorListeners;
using PDDL.Models;
using PDDL.Models.Problem;
using PDDL.Parsers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDL.PDDLSharp.Tests.System
{
    [TestClass]
    public class CodeGeneratorTests : BaseBenchmarkedTests
    {
        [ClassInitialize]
        public static async Task InitialiseAsync(TestContext context)
        {
            await Setup();
        }

        public static IEnumerable<object[]> GetDictionaryData()
        {
            foreach (var key in _testDict.Keys)
                yield return new object[] { key, _testDict[key] };
        }

        [TestMethod]
        [DynamicData(nameof(GetDictionaryData), DynamicDataSourceType.Method)]
        public void Can_Parse_Generate_Parse_Domain(string domain, List<string> problems)
        {
            Trace.WriteLine($"Domain: {new FileInfo(domain).Directory.Name}, problems: {problems.Count}");

            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IPDDLParser parser = GetParser(domain, listener);
            IPDDLCodeGenerator generator = new PDDLCodeGenerator(listener);

            // ACT
            var domainDecl = parser.ParseDomain(domain);
            generator.Generate(domainDecl, "temp.pddl");
            var newDomainDecl = parser.ParseDomain("temp.pddl");

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }

        [TestMethod]
        [DynamicData(nameof(GetDictionaryData), DynamicDataSourceType.Method)]
        public void Can_Parse_Generate_Parse_Problem(string domain, List<string> problems)
        {
            Trace.WriteLine($"Domain: {new FileInfo(domain).Directory.Name}, problems: {problems.Count}");

            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IPDDLParser parser = GetParser(domain, listener);
            IPDDLCodeGenerator generator = new PDDLCodeGenerator(listener);

            // ACT
            foreach (var problem in problems)
            {
                Trace.WriteLine($"   Testing problem: {problem}");

                var problemDecl = parser.ParseProblem(problem);
                generator.Generate(problemDecl, "temp.pddl");
                var newProblemDecl = parser.ParseProblem("temp.pddl");

                Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
                listener.Errors.Clear();
            }
            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }

        [TestMethod]
        [DynamicData(nameof(GetDictionaryData), DynamicDataSourceType.Method)]
        public void Can_Parse_Generate_Parse_Full(string domain, List<string> problems)
        {
            Trace.WriteLine($"Domain: {new FileInfo(domain).Directory.Name}, problems: {problems.Count}");

            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IPDDLParser parser = GetParser(domain, listener);
            IPDDLCodeGenerator generator = new PDDLCodeGenerator(listener);
            IContextualiser<PDDLDecl> contextualiser = new PDDLDeclContextualiser(listener);
            IAnalyser<PDDLDecl> analyser = new PDDLDeclAnalyser(listener);

            // ACT
            foreach (var problem in problems)
            {
                Trace.WriteLine($"   Testing problem: {problem}");

                var decl = new PDDLDecl(parser.ParseDomain(domain), parser.ParseProblem(problem));
                contextualiser.Contexturalise(decl);
                analyser.PostAnalyse(decl);
                generator.Generate(decl.Domain, "temp_domain.pddl");
                generator.Generate(decl.Problem, "temp_problem.pddl");
                var newDecl = new PDDLDecl(parser.ParseDomain("temp_domain.pddl"), parser.ParseProblem("temp_problem.pddl"));
                contextualiser.Contexturalise(newDecl);
                analyser.PostAnalyse(newDecl);

                Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
                listener.Errors.Clear();
            }
            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }
    }
}
