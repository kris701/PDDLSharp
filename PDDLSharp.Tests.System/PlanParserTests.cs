using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Parsers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.Plans;

namespace PDDLSharp.PDDLSharp.Tests.System
{
    [TestClass]
    public class PlanParserTests : BasePlanBenchmarkedTests
    {
        [ClassInitialize]
        public static async Task InitialiseAsync(TestContext context)
        {
            await Setup();
        }

        public static IEnumerable<object[]> GetDictionaryData()
        {
            foreach (var key in _testDict.Keys)
                if (_testPlanDict.ContainsKey(new FileInfo(key).Directory.Name))
                    yield return new object[] { key, _testDict[key], _testPlanDict[new FileInfo(key).Directory.Name] };
        }

        [TestMethod]
        [DynamicData(nameof(GetDictionaryData), DynamicDataSourceType.Method)]
        public void Can_ParsePlans(string domain, List<string> problems, List<string> plans)
        {
            Trace.WriteLine($"Domain: {new FileInfo(domain).Directory.Name}, problems: {problems.Count}");

            // ARRANGE
            Assert.AreEqual(problems.Count, plans.Count);
            IErrorListener listener = new ErrorListener();
            IParser<INode> parser = GetParser(domain, listener);
            IParser<ActionPlan> planParser = new FastDownwardPlanParser(listener);

            // ACT
            for(int i = 0; i < problems.Count; i++)
            {
                Trace.WriteLine($"   Parsing problem: {problems[i]}");
                var domainDecl = parser.ParseAs<DomainDecl>(domain);
                var problemDecl = parser.ParseAs<ProblemDecl>(problems[i]);
                Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
                Trace.WriteLine($"   Parsing plan: {plans[i]}");
                planParser.Parse(plans[i]);
                Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
                listener.Errors.Clear();
            }

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }
    }
}
