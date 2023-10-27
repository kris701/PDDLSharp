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
            IState state = new RelaxedPDDLStateSpace(decl);
            var actions = GetGroundedActions(decl);

            // ACT
            var graph = RelaxedPlanningGraph.GenerateRelaxedPlanningGraph(state, actions);

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
            IState state = new RelaxedPDDLStateSpace(decl);
            var actions = GetGroundedActions(decl);

            // ACT
            var graph = RelaxedPlanningGraph.GenerateRelaxedPlanningGraph(state, actions);

            // ASSERT
            Assert.AreEqual(expecteds.Length, graph.Count);
            for (int i = 0; i < expecteds.Length; i++)
                Assert.AreEqual(expecteds[i], graph[i].Actions.Count);
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
            IState state = new RelaxedPDDLStateSpace(decl);
            var actions = GetGroundedActions(decl);

            // ACT
            var graph = RelaxedPlanningGraph.GenerateRelaxedPlanningGraph(state, actions);

            // ASSERT
            Assert.AreEqual(0, graph[0].Actions.Count);
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
            IState state = new RelaxedPDDLStateSpace(decl);
            var actions = GetGroundedActions(decl);

            // ACT
            var graph = RelaxedPlanningGraph.GenerateRelaxedPlanningGraph(state, actions);

            // ASSERT
            Assert.AreEqual(decl.Problem.Init.Predicates.Count, graph[0].Propositions.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "No applicable actions found!")]
        public void Cant_GenerateGraph_IfNoApplicableActions_1()
        {
            // ARRANGE
            var decl = new PDDLDecl(new DomainDecl(), new ProblemDecl());
            decl.Problem.Goal = new GoalDecl();
            decl.Problem.Goal.GoalExp = new PredicateExp("abc");
            IState state = new RelaxedPDDLStateSpace(decl);

            // ACT
            RelaxedPlanningGraph.GenerateRelaxedPlanningGraph(state, new HashSet<ActionDecl>());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "No applicable actions found!")]
        public void Cant_GenerateGraph_IfNoApplicableActions_2()
        {
            // ARRANGE
            var decl = new PDDLDecl(new DomainDecl(), new ProblemDecl());
            decl.Problem.Goal = new GoalDecl();
            decl.Problem.Goal.GoalExp = new PredicateExp("abc");
            IState state = new RelaxedPDDLStateSpace(decl);

            var actions = new HashSet<ActionDecl>();
            var action = new ActionDecl("non-applicable");
            action.Parameters = new ParameterExp(new List<NameExp>() { new NameExp("?a") });
            action.Preconditions = new PredicateExp("wew", new List<NameExp>() { new NameExp("?a") });
            actions.Add(action);

            // ACT
            RelaxedPlanningGraph.GenerateRelaxedPlanningGraph(state, actions);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Relaxed state did not change!")]
        public void Cant_GenerateGraph_IfActionDoesNothing()
        {
            // ARRANGE
            var decl = new PDDLDecl(new DomainDecl(), new ProblemDecl());
            decl.Problem.Goal = new GoalDecl();
            decl.Problem.Goal.GoalExp = new PredicateExp("abc");
            IState state = new RelaxedPDDLStateSpace(decl);

            var actions = new HashSet<ActionDecl>();
            var action = new ActionDecl("non-applicable");
            action.Parameters = new ParameterExp(new List<NameExp>() { new NameExp("?a") });
            actions.Add(action);

            // ACT
            RelaxedPlanningGraph.GenerateRelaxedPlanningGraph(state, actions);
        }
    }
}
