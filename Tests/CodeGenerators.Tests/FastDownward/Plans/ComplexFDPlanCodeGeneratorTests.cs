using PDDLSharp;
using PDDLSharp.Analysers;
using PDDLSharp.CodeGenerators;
using PDDLSharp.CodeGenerators.FastDownward.Plans;
using PDDLSharp.Contextualisers;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
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

namespace PDDLSharp.CodeGenerators.Tests.FastDownward.Plans
{
    [TestClass]
    public class ComplexFDPlanCodeGeneratorTests : BenchmarkBuilder
    {
        public static IEnumerable<object[]> PlansData() => GetPlans();

        [TestMethod]
        [DynamicData(nameof(PlansData), DynamicDataSourceType.Method)]
        public void Can_Parse_Generate_Parse_Plan(string plan)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<ActionPlan> parser = new FDPlanParser(listener);
            var initialDecl = parser.Parse(new FileInfo(plan));
            ICodeGenerator<ActionPlan> generator = new FastDownwardPlanGenerator(listener);

            // ACT
            generator.Generate(initialDecl, "temp.plan");
            var newPlan = parser.Parse(new FileInfo("temp.plan"));
            Assert.IsTrue(initialDecl.Equals(newPlan));

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }
    }
}
