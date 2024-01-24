using PDDLSharp;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Toolkit;
using PDDLSharp.Toolkit.Planners;
using PDDLSharp.Toolkit.Planners.Heuristics;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.Planners.Search.BlackBox;
using PDDLSharp.Toolkit.Planners.Tests;
using PDDLSharp.Toolkit.Planners.Tests.Search;
using PDDLSharp.Toolkit.Planners.Tests.Search.BlackBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Tests.Search.BlackBox
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
        public void Can_FindSolution_hGoal(string domain, string problem)
        {
            // ARRANGE
            var decl = GetSASDecl(domain, problem);
            var planner = new GreedyBFS(decl, new hGoal());
            var validator = new PlanValidator.PlanValidator();

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
        public void Cant_FindSolution_hGoal_IfImpossible(string domain, string problem)
        {
            // ARRANGE
            var decl = GetSASDecl(domain, problem);
            decl.Goal.Clear();
            decl.Goal.Add(new Models.SAS.Fact("non-existent"));
            var planner = new GreedyBFS(decl, new hGoal());

            // ACT
            var result = planner.Solve();

            // ASSERT
            Assert.AreEqual(new ActionPlan(), result);
        }
    }
}
