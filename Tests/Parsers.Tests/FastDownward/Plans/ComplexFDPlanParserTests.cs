using PDDLSharp;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.FastDownward.Plans;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTools;

namespace PDDLSharp.Parsers.Tests.FastDownward.Plans
{
    [TestClass]
    public class ComplexFDPlanParserTests : BenchmarkBuilder
    {
        public static IEnumerable<object[]> PlansData() => GetPlans();

        [TestMethod]
        [DynamicData(nameof(PlansData), DynamicDataSourceType.Method)]
        public void Can_ParsePlans(string plan)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<ActionPlan> planParser = new FDPlanParser(listener);

            // ACT
            planParser.Parse(new FileInfo(plan));

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }
    }
}
