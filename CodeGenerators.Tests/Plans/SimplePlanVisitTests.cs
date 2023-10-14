using PDDLSharp;
using PDDLSharp.CodeGenerators;
using PDDLSharp.CodeGenerators.Tests;
using PDDLSharp.CodeGenerators.Tests.Plans;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.AST;
using PDDLSharp.Models.Plans;
using System;

namespace PDDLSharp.CodeGenerators.Tests.Plans
{
    [TestClass]
    public class SimplePlanVisitTests
    {
        public static IEnumerable<object[]> GetTestData()
        {
            yield return new object[]
            {
                new ActionPlan(new List<GroundedAction>()
                {

                },0),
                "; cost = 0 (unit cost)"
            };

            yield return new object[]
            {
                new ActionPlan(new List<GroundedAction>()
                {
                    new GroundedAction("action")
                },1),
                $"(action){Environment.NewLine}; cost = 1 (unit cost)"
            };

            yield return new object[]
            {
                new ActionPlan(new List<GroundedAction>()
                {
                    new GroundedAction("action"),
                    new GroundedAction("action2", "obja")
                },2),
                $"(action){Environment.NewLine}(action2 obja){Environment.NewLine}; cost = 2 (unit cost)"
            };

            yield return new object[]
            {
                new ActionPlan(new List<GroundedAction>()
                {
                    new GroundedAction("action"),
                    new GroundedAction("action2", "obja"),
                    new GroundedAction("action3", "obja", "objb")
                },3),
                $"(action){Environment.NewLine}(action2 obja){Environment.NewLine}(action3 obja objb){Environment.NewLine}; cost = 3 (unit cost)"
            };
        }

        // This is just a test with some random sets of nodes.
        // These tests are not exhaustive!
        [TestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void SmallNodeTests(ActionPlan node, string expected)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            ICodeGenerator<ActionPlan> generator = new FastDownwardPlanGenerator(listener);

            // ACT
            var result = generator.Generate(node);

            // ASSERT
            Assert.AreEqual(expected.Replace(Environment.NewLine, ""), result.Replace(Environment.NewLine, ""));
        }
    }
}