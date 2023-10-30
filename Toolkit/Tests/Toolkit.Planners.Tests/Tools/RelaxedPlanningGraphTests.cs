using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.SAS;
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
    public class RelaxedPlanningGraphTests : BasePlannerTests
    {
        [TestMethod]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob01.pddl", 3)]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob06.pddl", 3)]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p01.pddl", 5)]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p05.pddl", 7)]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s1-0.pddl", 4)]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s2-4.pddl", 4)]
        public void Can_GenerateGraph_Layer_Size(string domain, string problem, int expected)
        {
            // ARRANGE
            var decl = GetPDDLDecl(domain, problem);
            var state = new RelaxedSASStateSpace(decl);
            var actions = GetOperators(decl);
            var generator = new RelaxedPlanningGraph();

            // ACT
            var graph = generator.GenerateRelaxedPlanningGraph(state, actions);

            // ASSERT
            Assert.AreEqual(expected, graph.Count);
        }

        [TestMethod]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob01.pddl", 0, 6, 10)]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob06.pddl", 0, 30, 58)]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p01.pddl", 0, 8, 20, 14, 14)]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p05.pddl", 0, 9, 27, 42, 88, 223, 239)]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s1-0.pddl", 0, 1, 2, 1)]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s2-4.pddl", 0, 3, 11, 2)]
        public void Can_GenerateGraph_Layer_ActionSize(string domain, string problem, params int[] expecteds)
        {
            // ARRANGE
            var decl = GetPDDLDecl(domain, problem);
            var state = new RelaxedSASStateSpace(decl);
            var actions = GetOperators(decl);
            var generator = new RelaxedPlanningGraph();

            // ACT
            var graph = generator.GenerateRelaxedPlanningGraph(state, actions);

            // ASSERT
            Assert.AreEqual(expecteds.Length, graph.Count);
            for (int i = 0; i < expecteds.Length; i++)
                Assert.AreEqual(expecteds[i], graph[i].Operators.Count);
        }

        [TestMethod]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob01.pddl")]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob06.pddl")]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p01.pddl")]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p05.pddl")]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s1-0.pddl")]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s2-4.pddl")]
        public void Can_GenerateGraph_Layer_ActionSize_FirstAlwaysZero(string domain, string problem)
        {
            // ARRANGE
            var decl = GetPDDLDecl(domain, problem);
            var state = new RelaxedSASStateSpace(decl);
            var actions = GetOperators(decl);
            var generator = new RelaxedPlanningGraph();

            // ACT
            var graph = generator.GenerateRelaxedPlanningGraph(state, actions);

            // ASSERT
            Assert.AreEqual(0, graph[0].Operators.Count);
        }

        [TestMethod]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob01.pddl")]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob06.pddl")]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p01.pddl")]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p05.pddl")]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s1-0.pddl")]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s2-4.pddl")]
        public void Can_GenerateGraph_Layer_Proposition_FirstAlwaysInits(string domain, string problem)
        {
            // ARRANGE
            var decl = GetPDDLDecl(domain, problem);
            var state = new RelaxedSASStateSpace(decl);
            var actions = GetOperators(decl);
            var generator = new RelaxedPlanningGraph();

            // ACT
            var graph = generator.GenerateRelaxedPlanningGraph(state, actions);

            // ASSERT
            Assert.AreEqual(decl.Problem.Init.Predicates.Count, graph[0].Propositions.Count);
        }

        [TestMethod]
        public void Cant_GenerateGraph_IfNoApplicableActions_1()
        {
            // ARRANGE
            var decl = new PDDLDecl(new DomainDecl(), new ProblemDecl());
            decl.Problem.Goal = new GoalDecl();
            decl.Problem.Goal.GoalExp = new PredicateExp("abc");
            var state = new RelaxedSASStateSpace(decl);
            var generator = new RelaxedPlanningGraph();

            // ACT
            var result = generator.GenerateRelaxedPlanningGraph(state, new List<Operator>());

            // ASSERT
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Cant_GenerateGraph_IfNoApplicableActions_2()
        {
            // ARRANGE
            var decl = new PDDLDecl(new DomainDecl(), new ProblemDecl());
            decl.Problem.Goal = new GoalDecl();
            decl.Problem.Goal.GoalExp = new PredicateExp("abc");
            var state = new RelaxedSASStateSpace(decl);

            var actions = new List<Operator>()
            {
                new Operator(
                    "non-applicable", 
                    new string[]{ "?a" },
                    new Fact[]{ new Fact("wew", "?a") },
                    new Fact[]{ },
                    new Fact[]{ })
            };
            var generator = new RelaxedPlanningGraph();

            // ACT
            var result = generator.GenerateRelaxedPlanningGraph(state, actions);

            // ASSERT
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Cant_GenerateGraph_IfActionDoesNothing()
        {
            // ARRANGE
            var decl = new PDDLDecl(new DomainDecl(), new ProblemDecl());
            decl.Problem.Goal = new GoalDecl();
            decl.Problem.Goal.GoalExp = new PredicateExp("abc");
            var state = new RelaxedSASStateSpace(decl);

            var actions = new List<Operator>()
            {
                new Operator(
                    "non-applicable",
                    new string[]{ "?a" },
                    new Fact[]{ },
                    new Fact[]{ },
                    new Fact[]{ })
            };
            var generator = new RelaxedPlanningGraph();

            // ACT
            var result = generator.GenerateRelaxedPlanningGraph(state, actions);

            // ASSERT
            Assert.AreEqual(0, result.Count);
        }
    }
}
