using PDDLSharp;
using PDDLSharp.Analysers;
using PDDLSharp.Analysers.PDDL;
using PDDLSharp.CodeGenerators;
using PDDLSharp.CodeGenerators.PDDL;
using PDDLSharp.Contextualisers;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers;
using PDDLSharp.PDDLSharp;
using PDDLSharp.PDDLSharp.Tests;
using PDDLSharp.PDDLSharp.Tests.System;
using PDDLSharp.PDDLSharp.Tests.System.CodeGenerators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.PDDLSharp.Tests.System.CodeGenerators
{
    [TestClass]
    public class PDDLCodeGeneratorTests : BaseBenchmarkedTests
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
            IParser<INode> parser = GetParser(domain, listener);
            ICodeGenerator<INode> generator = new PDDLCodeGenerator(listener);

            // ACT
            var domainDecl = parser.ParseAs<DomainDecl>(domain);
            generator.Generate(domainDecl, "temp.pddl");
            var newDomainDecl = parser.ParseAs<DomainDecl>("temp.pddl");

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
            IParser<INode> parser = GetParser(domain, listener);
            ICodeGenerator<INode> generator = new PDDLCodeGenerator(listener);

            // ACT
            foreach (var problem in problems)
            {
                Trace.WriteLine($"   Testing problem: {problem}");

                var problemDecl = parser.ParseAs<ProblemDecl>(problem);
                generator.Generate(problemDecl, "temp.pddl");
                var newProblemDecl = parser.ParseAs<ProblemDecl>("temp.pddl");

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
            IParser<INode> parser = GetParser(domain, listener);
            ICodeGenerator<INode> generator = new PDDLCodeGenerator(listener);
            IContextualiser contextualiser = new PDDLContextualiser(listener);
            IAnalyser analyser = new PDDLAnalyser(listener);

            // ACT
            foreach (var problem in problems)
            {
                Trace.WriteLine($"   Testing problem: {problem}");

                var decl = new PDDLDecl(
                    parser.ParseAs<DomainDecl>(domain),
                    parser.ParseAs<ProblemDecl>(problem)
                    );
                contextualiser.Contexturalise(decl);
                analyser.Analyse(decl);
                generator.Generate(decl.Domain, "temp_domain.pddl");
                generator.Generate(decl.Problem, "temp_problem.pddl");
                var newDecl = new PDDLDecl(parser.ParseAs<DomainDecl>("temp_domain.pddl"), parser.ParseAs<ProblemDecl>("temp_problem.pddl"));
                contextualiser.Contexturalise(newDecl);
                analyser.Analyse(newDecl);

                Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
                listener.Errors.Clear();
            }
            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }
    }
}
