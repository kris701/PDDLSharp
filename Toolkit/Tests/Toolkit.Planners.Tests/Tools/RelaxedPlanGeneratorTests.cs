using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.PDDL;
using PDDLSharp.Toolkit.Grounders;
using PDDLSharp.Toolkit.Planners.Tools;
using PDDLSharp.Toolkit.StateSpace;
using PDDLSharp.Toolkit.StateSpace.SAS;
using PDDLSharp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var state = new RelaxedSASStateSpace(decl);
            var actions = GetOperators(decl);
            var generator = new OperatorRPG(decl);

            // ACT
            var result = generator.GenerateReplaxedPlan(state, actions);

            // ASSERT
            Assert.IsFalse(generator.Failed);
            Assert.IsFalse(state.IsInGoal());
            foreach (var item in result)
                state.ExecuteNode(item);
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
            var state = new RelaxedSASStateSpace(decl);
            var operators = GetOperators(decl);
            var generator = new OperatorRPG(decl);

            // ACT
            var result = generator.GenerateReplaxedPlan(state, operators);

            // ASSERT
            Assert.IsFalse(generator.Failed);
            Assert.AreEqual(expected, result.Count);
        }
    }
}
