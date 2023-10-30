﻿using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Toolkit.Planners.Exceptions;
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
    public class GreedyBFSDHETests : BasePlannerTests
    {
        [TestMethod]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob01.pddl")]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob06.pddl")]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p01.pddl")]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s1-0.pddl")]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s2-4.pddl")]
        public void Can_FindSolution_hDepth(string domain, string problem)
        {
            // ARRANGE
            var decl = GetPDDLDecl(domain, problem);
            IPlanner planner = new GreedyBFSDHE(decl);
            planner.Operators = GetGroundedActions(decl);
            var h = new hDepth();
            IPlanValidator validator = new PlanValidator.PlanValidator();

            // ACT
            var result = planner.Solve(h);

            // ASSERT
            Assert.IsTrue(validator.Validate(result, decl));
        }

        [TestMethod]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob01.pddl")]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob06.pddl")]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p01.pddl")]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s1-0.pddl")]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s2-4.pddl")]
        public void Can_PreProcess(string domain, string problem)
        {
            // ARRANGE
            var decl = GetPDDLDecl(domain, problem);
            IPlanner planner = new GreedyBFSDHE(decl);
            var expected = GetGroundedActions(decl);

            // ACT
            planner.PreProcess();

            // ASSERT
            Assert.AreEqual(expected.Count, planner.Operators.Count);
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
            IPlanner planner = new GreedyBFSDHE(decl);
            planner.Operators = GetGroundedActions(decl);
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
        [ExpectedException(typeof(NoSolutionFoundException))]
        public void Cant_FindSolution_hDepth_IfImpossible(string domain, string problem)
        {
            // ARRANGE
            var decl = GetPDDLDecl(domain, problem);
            decl.Problem.Goal.GoalExp = new PredicateExp("non-existent");
            IPlanner planner = new GreedyBFSDHE(decl);
            planner.Operators = GetGroundedActions(decl);
            var h = new hDepth();

            // ACT
            var result = planner.Solve(h);
        }
    }
}