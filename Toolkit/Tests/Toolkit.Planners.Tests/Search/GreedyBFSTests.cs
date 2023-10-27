﻿using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Toolkit.Planners.Heuristics;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.PlanValidator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Tests.Search
{
    [TestClass]
    public class GreedyBFSTests : BasePlannerTests
    {
        [TestMethod]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob01.pddl")]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob06.pddl")]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p01.pddl")]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s1-0.pddl")]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s2-4.pddl")]
        public void Can_FindSolution_hBlind(string domain, string problem)
        {
            // ARRANGE
            var decl = GetPDDLDecl(domain, problem);
            IPlanner planner = new GreedyBFS(decl);
            planner.GroundedActions = GetGroundedActions(decl);
            var h = new hBlind(decl);
            IPlanValidator validator = new PlanValidator.PlanValidator();

            // ACT
            var result = planner.Solve(h);

            // ASSERT
            Assert.IsTrue(validator.Validate(result, decl));
        }

        [TestMethod]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob01.pddl")]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p01.pddl")]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s1-0.pddl")]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s2-4.pddl")]
        public void Can_FindSolution_hFF(string domain, string problem)
        {
            // ARRANGE
            var decl = GetPDDLDecl(domain, problem);
            IPlanner planner = new GreedyBFS(decl);
            planner.GroundedActions = GetGroundedActions(decl);
            var h = new hFF(decl);
            IPlanValidator validator = new PlanValidator.PlanValidator();

            // ACT
            var result = planner.Solve(h);

            // ASSERT
            Assert.IsTrue(validator.Validate(result, decl));
        }

        [TestMethod]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob01.pddl")]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p01.pddl")]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s1-0.pddl")]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s2-4.pddl")]
        [ExpectedException(typeof(Exception), "No solution found!")]
        public void Cant_FindSolution_hBlind_IfImpossible(string domain, string problem)
        {
            // ARRANGE
            var decl = GetPDDLDecl(domain, problem);
            decl.Problem.Goal.GoalExp = new PredicateExp("non-existent");
            IPlanner planner = new GreedyBFS(decl);
            planner.GroundedActions = GetGroundedActions(decl);
            var h = new hBlind(decl);

            // ACT
            var result = planner.Solve(h);
        }
    }
}