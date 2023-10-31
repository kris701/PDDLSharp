using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.PDDL;
using PDDLSharp.Toolkit.Planners.Heuristics;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.StateSpace;
using PDDLSharp.Toolkit.StateSpace.SAS;
using PDDLSharp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Tests.Heuristics
{
    [TestClass]
    public class hFFTests : BasePlannerTests
    {
        [TestMethod]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob01.pddl", 5)]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob06.pddl", 29)]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p01.pddl", 10)]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p05.pddl", 37)]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s1-0.pddl", 3)]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s2-4.pddl", 6)]
        public void Can_GeneratehFFCorrectly_FromInitialState(string domain, string problem, int expected)
        {
            // ARRANGE
            var decl = GetSASDecl(domain, problem);
            var h = new hFF(decl);
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
        public void Can_GeneratehFFCorrectly_FromGoalState(string domain, string problem, int expected)
        {
            // ARRANGE
            var decl = GetSASDecl(domain, problem);
            var h = new hFF(decl);
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
