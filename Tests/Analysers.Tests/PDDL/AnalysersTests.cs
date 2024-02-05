using PDDLSharp;
using PDDLSharp.Analysers;
using PDDLSharp.Analysers.PDDL;
using PDDLSharp.Contextualisers;
using PDDLSharp.Contextualisers.PDDL;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTools;

namespace PDDLSharp.Analysers.Tests.PDDL.Visitors
{
    [TestClass]
    public class AnalysersTests : BenchmarkBuilder
    {
        public static IEnumerable<object[]> DomainProblemData() => GetDomainsAndProblems();

        [TestMethod]
        [DynamicData(nameof(DomainProblemData), DynamicDataSourceType.Method)]
        public void Can_ParseProblemAndDomain_Analyse_STRIPS(string domain, string problem)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IAnalyser<PDDLDecl> analyser = new PDDLAnalyser(listener);
            var decl = GetPDDLDecl(domain, problem);

            // ACT
            analyser.Analyse(decl);

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }
    }
}
