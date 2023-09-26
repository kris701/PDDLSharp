using PDDL.Contextualisers;
using PDDL.ErrorListeners;
using PDDL.Models;
using PDDL.Models.Domain;
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
    public class ContextualiserTests : BaseBenchmarkedTests
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
        public void Can_ParseDomains_Contextualize_STRIPS(string domain, List<string> problems)
        {
            Trace.WriteLine($"Domain: {new FileInfo(domain).Directory.Name}, problems: {problems.Count}");

            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IPDDLParser parser = GetParser(domain, listener);
            IContextualiser<DomainDecl> contextualiser = new PDDLDomainDeclContextualiser(listener);

            // ACT
            var decl = parser.ParseDomain(domain);
            contextualiser.Contexturalise(decl);

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }

        [TestMethod]
        [DynamicData(nameof(GetDictionaryData), DynamicDataSourceType.Method)]
        public void Can_ParseProblems_Contextualise_STRIPS(string domain, List<string> problems)
        {
            Trace.WriteLine($"Domain: {new FileInfo(domain).Directory.Name}, problems: {problems.Count}");

            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IPDDLParser parser = GetParser(domain, listener);
            IContextualiser<ProblemDecl> contextualiser = new PDDLProblemDeclContextualiser(listener);

            // ACT
            foreach (var problem in problems)
            {
                Trace.WriteLine($"   Parsing problem: {problem}");
                var decl = parser.ParseProblem(problem);
                contextualiser.Contexturalise(decl);
                Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
                listener.Errors.Clear();
            }

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }

        [TestMethod]
        [DynamicData(nameof(GetDictionaryData), DynamicDataSourceType.Method)]
        public void Can_ParseProblemAndDomain_Contextualise_STRIPS(string domain, List<string> problems)
        {
            Trace.WriteLine($"Domain: {new FileInfo(domain).Directory.Name}, problems: {problems.Count}");

            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IPDDLParser parser = GetParser(domain, listener);
            IContextualiser<PDDLDecl> contextualiser = new PDDLDeclContextualiser(listener);
            Random rnd = new Random();

            // ACT
            foreach (var problem in problems)
            {
                Trace.WriteLine($"   Parsing problem: {problem}");
                var domainDecl = parser.ParseDomain(domain);
                var problemDecl = parser.ParseProblem(problem);
                var decl = new PDDLDecl(domainDecl, problemDecl);
                contextualiser.Contexturalise(decl);
                Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
                listener.Errors.Clear();
            }

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }
    }
}
