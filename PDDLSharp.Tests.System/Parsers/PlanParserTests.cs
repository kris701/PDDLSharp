using PDDLSharp;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.Plans;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.Plans;
using PDDLSharp.PDDLSharp;
using PDDLSharp.PDDLSharp.Tests;
using PDDLSharp.PDDLSharp.Tests.System;
using PDDLSharp.PDDLSharp.Tests.System.Parsers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.PDDLSharp.Tests.System.Parsers
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
            {
                if (_testPlanDict.ContainsKey(new FileInfo(key).Directory.Name))
                    yield return new object[] { key, _testDict[key], _testPlanDict[new FileInfo(key).Directory.Name] };
                else
                    yield return new object[] { key, _testDict[key], new List<string>() };
            }
        }

        [TestMethod]
        [DynamicData(nameof(GetDictionaryData), DynamicDataSourceType.Method)]
        public void Can_ParsePlans(string domain, List<string> problems, List<string> plans)
        {
            Trace.WriteLine($"Domain: {new FileInfo(domain).Directory.Name}, problems: {problems.Count}");

            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<INode> parser = GetParser(domain, listener);
            IParser<ActionPlan> planParser = new FastDownwardPlanParser(listener);

            // ACT
            bool any = false;
            foreach (var problem in problems)
            {
                var targetPlanStr = new FileInfo(problem).Name.Replace(".pddl", ".plan");
                var targetPlan = plans.FirstOrDefault(x => x.EndsWith(targetPlanStr));

                if (targetPlan != null)
                {
                    Trace.WriteLine($"   Parsing problem: {problem}");
                    var domainDecl = parser.ParseAs<DomainDecl>(domain);
                    var problemDecl = parser.ParseAs<ProblemDecl>(problem);
                    Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));

                    Trace.WriteLine($"   Parsing plan: {targetPlan}");
                    var plan = planParser.Parse(targetPlan);
                    Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
                    listener.Errors.Clear();
                    any = true;
                }
            }
            if (!any)
                Assert.Inconclusive($"Could not find any plans for the domain+problems!");

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }
    }
}
