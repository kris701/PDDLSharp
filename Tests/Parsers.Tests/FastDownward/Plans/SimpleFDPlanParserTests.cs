using PDDLSharp;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.FastDownward.Plans;
using PDDLSharp.Parsers.Tests;
using PDDLSharp.Parsers.Tests.FastDownward.Plans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Parsers.Tests.FastDownward.Plans
{
    [TestClass]
    public class SimpleFDPlanParserTests
    {
        [TestMethod]
        [DataRow("PlanTestData/plan1.plan", 0)]
        [DataRow("PlanTestData/plan2.plan", 1)]
        [DataRow("PlanTestData/plan3.plan", 10)]
        [DataRow("PlanTestData/plan4.plan", 10)]
        public void Can_ParsePlan_ActionCount(string testFile, int expectedActionCount)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<ActionPlan> parser = new FDPlanParser(listener);

            // ACT
            var res = parser.Parse(new FileInfo(testFile));

            // ASSERT
            Assert.AreEqual(expectedActionCount, res.Plan.Count);
        }

        [TestMethod]
        [DataRow("PlanTestData/plan1.plan", 0)]
        [DataRow("PlanTestData/plan2.plan", 1)]
        [DataRow("PlanTestData/plan3.plan", 10)]
        [DataRow("PlanTestData/plan4.plan", 50)]
        public void Can_ParsePlan_Cost(string testFile, int expectedCost)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<ActionPlan> parser = new FDPlanParser(listener);

            // ACT
            var res = parser.Parse(new FileInfo(testFile));

            // ASSERT
            Assert.AreEqual(expectedCost, res.Cost);
        }

        [TestMethod]
        [DataRow("PlanTestData/plan1.plan")]
        [DataRow("PlanTestData/plan2.plan", "lift")]
        [DataRow("PlanTestData/plan3.plan", "lift", "load", "drive", "lift", "load", "unload", "drive", "unload", "drop", "drop")]
        [DataRow("PlanTestData/plan4.plan", "lift", "load", "drive", "lift", "load", "unload", "drive", "unload", "drop", "drop")]
        public void Can_ParsePlan_ActionNames(string testFile, params string[] expectedActionNames)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<ActionPlan> parser = new FDPlanParser(listener);

            // ACT
            var res = parser.Parse(new FileInfo(testFile));

            // ASSERT
            Assert.AreEqual(expectedActionNames.Length, res.Plan.Count);
            for (int i = 0; i < res.Plan.Count; i++)
                Assert.AreEqual(expectedActionNames[i], res.Plan[i].ActionName);
        }

        [TestMethod]
        [DataRow("PlanTestData/plan2.plan", 0, "hoist0", "crate1", "pallet0", "depot0")]
        [DataRow("PlanTestData/plan3.plan", 2, "truck1", "depot0", "distributor0")]
        [DataRow("PlanTestData/plan4.plan", 9, "hoist2", "crate0", "pallet2", "distributor1")]
        public void Can_ParsePlan_ActionObjectNames(string testFile, int id, params string[] expectedActionNames)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<ActionPlan> parser = new FDPlanParser(listener);

            // ACT
            var res = parser.Parse(new FileInfo(testFile));

            // ASSERT
            Assert.IsTrue(res.Plan.Count > id);
            Assert.AreEqual(expectedActionNames.Length, res.Plan[id].Arguments.Count);
            for (int i = 0; i < expectedActionNames.Length; i++)
                Assert.AreEqual(expectedActionNames[i], res.Plan[id].Arguments[i].Name);
        }
    }
}
