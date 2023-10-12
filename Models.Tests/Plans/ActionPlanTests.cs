using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.Plans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.Tests.Plans
{
    [TestClass]
    public class ActionPlanTests
    {
        public static IEnumerable<object[]> GetIsEqualData()
        {
            yield return new object[] {
                new ActionPlan(new List<GroundedAction>(), 1),
                new ActionPlan(new List<GroundedAction>(), 1),
                true
            };

            yield return new object[] {
                new ActionPlan(new List<GroundedAction>(), 1),
                new ActionPlan(new List<GroundedAction>(), 2),
                false
            };

            yield return new object[] {
                new ActionPlan(new List<GroundedAction>(){
                    new GroundedAction("a")
                }, 1),
                new ActionPlan(new List<GroundedAction>(), 1),
                false
            };

            yield return new object[] {
                new ActionPlan(new List<GroundedAction>(){
                    new GroundedAction("a")
                }, 1),
                new ActionPlan(new List<GroundedAction>(){
                    new GroundedAction("a")
                }, 1),
                true
            };

            yield return new object[] {
                new ActionPlan(new List<GroundedAction>(){
                    new GroundedAction("a"),
                    new GroundedAction("b")
                }, 1),
                new ActionPlan(new List<GroundedAction>(){
                    new GroundedAction("b"),
                    new GroundedAction("a")
                }, 1),
                false
            };
        }

        [TestMethod]
        [DynamicData(nameof(GetIsEqualData), DynamicDataSourceType.Method)]
        public void Can_IsEqual(ActionPlan plana, ActionPlan planb, bool expected)
        {
            // ARRANGE
            // ACT
            var res = plana.Equals(planb);

            // ASSERT
            Assert.AreEqual(expected, res);
            if (expected)
                Assert.AreEqual(plana.GetHashCode(), planb.GetHashCode());
            else
                Assert.AreNotEqual(plana.GetHashCode(), planb.GetHashCode());
        }
    }
}
