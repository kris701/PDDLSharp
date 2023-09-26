using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDDL.Analysers;
using PDDL.Contextualisers;
using PDDL.ErrorListeners;
using PDDL.Models;
using PDDL.Models.Domain;
using PDDL.Models.Problem;
using PDDL.Parsers;
using System;
using System.Diagnostics;

namespace PDDL.PDDLSharp.Tests.System
{
    [TestClass]
    public class ParserTests : BaseBenchmarkedTests
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
        public void Can_ParseDomains_ParseOnly_STRIPS(string domain, List<string> problems)
        {
            Trace.WriteLine($"Domain: {new FileInfo(domain).Directory.Name}, problems: {problems.Count}");

            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IPDDLParser parser = GetParser(domain, listener);

            // ACT
            parser.ParseDomain(domain);

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }

        [TestMethod]
        [DynamicData (nameof(GetDictionaryData), DynamicDataSourceType.Method)]
        public void Can_ParseProblems_ParseOnly_STRIPS(string domain, List<string> problems)
        {
            Trace.WriteLine($"Domain: {new FileInfo(domain).Directory.Name}, problems: {problems.Count}");

            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IPDDLParser parser = GetParser(domain, listener);
            Random rnd = new Random();

            // ACT
            foreach (var problem in problems.OrderBy(x => rnd.Next()))
            {
                if (new FileInfo(problem).Length < MaxFileSize)
                {
                    Trace.WriteLine($"   Parsing problem: {problem}");
                    var decl = parser.ParseProblem(problem);
                    Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
                    listener.Errors.Clear();
                }
            }

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }

        [TestMethod]
        [DynamicData(nameof(GetDictionaryData), DynamicDataSourceType.Method)]
        public void Can_ParseProblemAndDomain_ParseOnly_STRIPS(string domain, List<string> problems)
        {
            Trace.WriteLine($"Domain: {new FileInfo(domain).Directory.Name}, problems: {problems.Count}");

            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IPDDLParser parser = GetParser(domain, listener);
            Random rnd = new Random();

            // ACT
            foreach (var problem in problems.OrderBy(x => rnd.Next()))
            {
                if (new FileInfo(problem).Length < MaxFileSize)
                {
                    Trace.WriteLine($"   Parsing problem: {problem}");
                    var domainDecl = parser.ParseDomain(domain);
                    var problemDecl = parser.ParseProblem(problem);
                    Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
                    listener.Errors.Clear();
                }
            }

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }
    }
}