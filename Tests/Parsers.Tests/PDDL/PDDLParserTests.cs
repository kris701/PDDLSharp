using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDDLSharp;
using PDDLSharp.Analysers;
using PDDLSharp.Contextualisers;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.PDDL;
using System;
using System.Diagnostics;
using TestTools;

namespace PDDLSharp.Parsers.Tests.PDDL
{
    [TestClass]
    public class PDDLParserTests : BenchmarkBuilder
    {
        public static IEnumerable<object[]> DomainsData() => GetDomains();
        public static IEnumerable<object[]> ProblemsData() => GetProblems();

        [TestMethod]
        [DynamicData(nameof(DomainsData), DynamicDataSourceType.Method)]
        public void Can_ParseDomains_ParseOnly_STRIPS(string domainFile)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<INode> parser = new PDDLParser(listener);

            // ACT
            parser.ParseAs<DomainDecl>(new FileInfo(domainFile));

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }

        [TestMethod]
        [DynamicData(nameof(ProblemsData), DynamicDataSourceType.Method)]
        public void Can_ParseProblems_ParseOnly_STRIPS(string problemFile)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<INode> parser = new PDDLParser(listener);

            // ACT
            var decl = parser.ParseAs<ProblemDecl>(new FileInfo(problemFile));

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }
    }
}