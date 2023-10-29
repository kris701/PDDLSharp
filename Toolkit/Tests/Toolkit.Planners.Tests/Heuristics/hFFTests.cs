using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.PDDL;
using PDDLSharp.Toolkit.Grounders;
using PDDLSharp.Toolkit.Planners.Heuristics;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.StateSpace;
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
            var decl = GetPDDLDecl(domain, problem);
            IHeuristic h = new hFF(decl);
            IState state = new PDDLStateSpace(decl);
            var actions = GetGroundedActions(decl);

            // ACT
            var newValue = h.GetValue(new StateMove(), state, actions);

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
            var decl = GetPDDLDecl(domain, problem);
            IHeuristic h = new hFF(decl);
            IState state = new PDDLStateSpace(decl);
            state.ExecuteNode(decl.Problem.Goal.GoalExp);
            var actions = GetGroundedActions(decl);

            // ACT
            var newValue = h.GetValue(new StateMove(), state, actions);

            // ASSERT
            Assert.AreEqual(expected, newValue);
        }
    }
}
