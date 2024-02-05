using PDDLSharp;
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

namespace PDDLSharp.Contextualisers.Tests.PDDL
{
    [TestClass]
    public class ComplexPDDLContextualiserTests : BenchmarkBuilder
    {
        public static IEnumerable<object[]> GetPDDLData() => GetDomainsAndProblems();

        [TestMethod]
        [DynamicData(nameof(GetPDDLData), DynamicDataSourceType.Method)]
        public void Can_ParseProblemAndDomain_Contextualise_STRIPS(string domain, string problem)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IContextualiser contextualiser = new PDDLContextualiser(listener);

            // ACT
            var decl = GetPDDLDecl(domain, problem);
            contextualiser.Contexturalise(decl);

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }
    }
}
