﻿using PDDLSharp;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Toolkit;
using PDDLSharp.Toolkit.Planners;
using PDDLSharp.Toolkit.Planners.Heuristics;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.Planners.Search.Classical;
using PDDLSharp.Toolkit.Planners.Tests;
using PDDLSharp.Toolkit.Planners.Tests.Search;
using PDDLSharp.Toolkit.Planners.Tests.Search.Classical;
using PDDLSharp.Toolkit.PlanValidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Tests.Search.Classical
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
            var decl = GetSASDecl(domain, problem);
            var planner = new GreedyBFSDHE(decl, new hDepth());
            var validator = new PlanValidators.PlanValidator();

            // ACT
            var result = planner.Solve();

            // ASSERT
            Assert.IsTrue(validator.Validate(result, GetPDDLDecl(domain, problem)));
        }

        [TestMethod]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob01.pddl")]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p01.pddl")]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s1-0.pddl")]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s2-4.pddl")]
        public void Can_FindSolution_hFF(string domain, string problem)
        {
            // ARRANGE
            var decl = GetSASDecl(domain, problem);
            var planner = new GreedyBFSDHE(decl, new hFF(decl));
            var validator = new PlanValidators.PlanValidator();

            // ACT
            var result = planner.Solve();

            // ASSERT
            Assert.IsTrue(validator.Validate(result, GetPDDLDecl(domain, problem)));
        }

        [TestMethod]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob01.pddl")]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p01.pddl")]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s1-0.pddl")]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s2-4.pddl")]
        public void Cant_FindSolution_hDepth_IfImpossible(string domain, string problem)
        {
            // ARRANGE
            var decl = GetSASDecl(domain, problem);
            decl.Goal.Clear();
            decl.Goal.Add(new Models.SAS.Fact("non-existent"));
            var planner = new GreedyBFSDHE(decl, new hDepth());

            // ACT
            var result = planner.Solve();

            // ASSERT
            Assert.AreEqual(new ActionPlan(), result);
        }
    }
}
