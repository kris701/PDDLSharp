using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDDLSharp.Analysers;
using PDDLSharp.Contextualisers;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.Domain;
using PDDLSharp.Models.Problem;
using PDDLSharp.Parsers;
using System;
using System.Diagnostics;

namespace PDDLSharp.PDDLSharp.Tests.System
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
            IParser<INode> parser = GetParser(domain, listener);

            // ACT
            parser.ParseAs<DomainDecl>(domain);

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
            IParser<INode> parser = GetParser(domain, listener);

            // ACT
            foreach (var problem in problems)
            {
                Trace.WriteLine($"   Parsing problem: {problem}");
                var decl = parser.ParseAs<ProblemDecl>(problem);
                Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
                listener.Errors.Clear();
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
            IParser<INode> parser = GetParser(domain, listener);

            // ACT
            foreach (var problem in problems)
            {
                Trace.WriteLine($"   Parsing problem: {problem}");
                var domainDecl = parser.ParseAs<DomainDecl>(domain);
                var problemDecl = parser.ParseAs<ProblemDecl>(problem);
                Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
                listener.Errors.Clear();
            }

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }
    }
}