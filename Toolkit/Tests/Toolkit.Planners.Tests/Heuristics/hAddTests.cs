﻿using PDDLSharp.Toolkit.Planners.Heuristics;
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
    public class hAddTests : BasePlannerTests
    {
        [TestMethod]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob01.pddl", 6)]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob06.pddl", 42)]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p01.pddl", 2)]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p05.pddl", 9)]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s1-0.pddl", 7)]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s2-4.pddl", 14)]
        public void Can_GeneratehAddCorrectly_FromInitialState(string domain, string problem, int expected)
        {
            // ARRANGE
            var decl = GetPDDLDecl(domain, problem);
            var h = new hAdd();
            var state = new SASStateSpace(decl);
            var actions = GetOperators(decl);

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
        public void Can_GeneratehAddCorrectly_FromGoalState(string domain, string problem, int expected)
        {
            // ARRANGE
            var decl = GetPDDLDecl(domain, problem);
            var h = new hAdd();
            var state = new SASStateSpace(decl);
            foreach (var goal in state.Goals)
                state.Add(goal);
            var actions = GetOperators(decl);

            // ACT
            var newValue = h.GetValue(new StateMove(), state, actions);

            // ASSERT
            Assert.AreEqual(expected, newValue);
        }
    }
}
