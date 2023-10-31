using PDDLSharp.Toolkit.Planners.Heuristics;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.StateSpace.SAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Tests.Heuristics
{
    [TestClass]
    public class hMaxTests : BasePlannerTests
    {
        [TestMethod]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob01.pddl", 2)]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob06.pddl", 2)]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p01.pddl", 4)]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p05.pddl", 6)]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s1-0.pddl", 3)]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s2-4.pddl", 3)]
        public void Can_GeneratehMaxCorrectly_FromInitialState(string domain, string problem, int expected)
        {
            // ARRANGE
            var decl = GetSASDecl(domain, problem);
            var h = new hMax();
            var state = new SASStateSpace(decl);

            // ACT
            var newValue = h.GetValue(new StateMove(), state, decl.Operators);

            // ASSERT
            Assert.AreEqual(expected, newValue);
        }

        [TestMethod]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob01.pddl", 0)]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob06.pddl", 0)]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p01.pddl", 0)]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p05.pddl", 0)]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s1-0.pddl", 0)]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s2-4.pddl", 0)]
        public void Can_GeneratehMaxCorrectly_FromGoalState(string domain, string problem, int expected)
        {
            // ARRANGE
            var decl = GetSASDecl(domain, problem);
            var h = new hMax();
            var state = new SASStateSpace(decl);
            foreach (var goal in decl.Goal)
                state.Add(goal);

            // ACT
            var newValue = h.GetValue(new StateMove(), state, decl.Operators);

            // ASSERT
            Assert.AreEqual(expected, newValue);
        }
    }
}
