using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models;
using PDDLSharp.Parsers.PDDL;
using PDDLSharp.Parsers;
using PDDLSharp.Toolkit.Grounders;
using PDDLSharp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Toolkit.StateSpace;
using PDDLSharp.Toolkit.Planners.Tools;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Toolkit.Planners.Tests.Tools
{
    [TestClass]
    public class RelaxedPlanGeneratorTests : BasePlannerTests
    {
        [TestMethod]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob01.pddl")]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob06.pddl")]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p01.pddl")]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p05.pddl")]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s1-0.pddl")]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s2-4.pddl")]
        public void Can_GenerateRelaxedPlan_ResultsInGoal(string domain, string problem)
        {
            // ARRANGE
            var decl = GetPDDLDecl(domain, problem);
            IState state = new RelaxedPDDLStateSpace(decl);
            var actions = GetGroundedActions(decl);
            var generator = new RelaxedPlanGenerator(decl);

            // ACT
            var result = generator.GenerateReplaxedPlan(state, actions);

            // ASSERT
            Assert.IsFalse(state.IsInGoal());
            foreach (var item in result)
                state.ExecuteNode(item.Effects);
            Assert.IsTrue(state.IsInGoal());
        }

        [TestMethod]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob01.pddl", 5)]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob06.pddl", 29)]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p01.pddl", 10)]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p05.pddl", 37)]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s1-0.pddl", 3)]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s2-4.pddl", 6)]
        public void Can_GenerateRelaxedPlan_Length(string domain, string problem, int expected)
        {
            // ARRANGE
            var decl = GetPDDLDecl(domain, problem);
            IState state = new RelaxedPDDLStateSpace(decl);
            var actions = GetGroundedActions(decl);
            var generator = new RelaxedPlanGenerator(decl);

            // ACT
            var result = generator.GenerateReplaxedPlan(state, actions);

            // ASSERT
            Assert.AreEqual(expected, result.Count);
        }
    }
}
