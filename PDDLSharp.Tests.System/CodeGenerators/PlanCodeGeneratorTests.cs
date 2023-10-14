using PDDLSharp;
using PDDLSharp.Analysers;
using PDDLSharp.CodeGenerators;
using PDDLSharp.Contextualisers;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.Plans;
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
    public class PlanCodeGeneratorTests : BasePlanBenchmarkedTests
    {
        [ClassInitialize]
        public static async Task InitialiseAsync(TestContext context)
        {
            await Setup();
        }

        public static IEnumerable<object[]> GetDictionaryData()
        {
            foreach (var key in _testPlanDict.Keys)
                yield return new object[] { key, _testPlanDict[key] };
        }

        [TestMethod]
        [DynamicData(nameof(GetDictionaryData), DynamicDataSourceType.Method)]
        public void Can_Parse_Generate_Parse_Domain(string domain, List<string> plans)
        {
            Trace.WriteLine($"Domain: {new FileInfo(domain).Directory.Name}, plans: {plans.Count}");

            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<ActionPlan> parser = new FastDownwardPlanParser(listener);
            ICodeGenerator<ActionPlan> generator = new FastDownwardPlanGenerator(listener);

            // ACT
            foreach(var plan in plans)
            {
                Trace.WriteLine($"Testing plan '{plan}'");
                var orgPlan = parser.Parse(plan);
                generator.Generate(orgPlan, "temp.plan");
                var newPlan = parser.Parse("temp.plan");
                Assert.IsTrue(orgPlan.Equals(newPlan));
            }

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }
    }
}
