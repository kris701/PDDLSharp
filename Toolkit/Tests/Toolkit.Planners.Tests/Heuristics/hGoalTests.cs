using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Toolkit.Planners.Heuristics;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.StateSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Tests.Heuristics
{
    [TestClass]
    public class hGoalTests
    {
        [TestMethod]
        public void Can_GeneratehGoalCorrectly_NoGoals()
        {
            // ARRANGE
            var decl = new PDDLDecl(new DomainDecl(), new ProblemDecl());
            decl.Problem.Goal = new GoalDecl();
            decl.Problem.Goal.GoalExp = new PredicateExp("goal-fact");
            IHeuristic h = new hGoal(decl);
            var parent = new StateMove();
            var state = new PDDLStateSpace(decl);

            // ACT
            var newValue = h.GetValue(parent, state, new List<ActionDecl>());

            // ASSERT
            Assert.AreEqual(1, newValue);
        }

        [TestMethod]
        public void Can_GeneratehGoalCorrectly_OneGoal()
        {
            // ARRANGE
            var decl = new PDDLDecl(new DomainDecl(), new ProblemDecl());
            decl.Problem.Goal = new GoalDecl();
            decl.Problem.Goal.GoalExp = new PredicateExp("goal-fact");
            decl.Problem.Init = new InitDecl();
            decl.Problem.Init.Predicates.Add(new PredicateExp("goal-fact"));
            IHeuristic h = new hGoal(decl);
            var parent = new StateMove();
            var state = new PDDLStateSpace(decl);

            // ACT
            var newValue = h.GetValue(parent, state, new List<ActionDecl>());

            // ASSERT
            Assert.AreEqual(0, newValue);
        }

        [TestMethod]
        public void Can_GeneratehGoalCorrectly_MultipleGoals_1()
        {
            // ARRANGE
            var decl = new PDDLDecl(new DomainDecl(), new ProblemDecl());
            decl.Problem.Goal = new GoalDecl();
            decl.Problem.Goal.GoalExp = new AndExp(new List<IExp>() { new PredicateExp("goal-fact-1"), new PredicateExp("goal-fact-2"), new PredicateExp("goal-fact-3") });
            decl.Problem.Init = new InitDecl();
            decl.Problem.Init.Predicates.Add(new PredicateExp("goal-fact-1"));
            decl.Problem.Init.Predicates.Add(new PredicateExp("goal-fact-2"));
            IHeuristic h = new hGoal(decl);
            var parent = new StateMove();
            var state = new PDDLStateSpace(decl);

            // ACT
            var newValue = h.GetValue(parent, state, new List<ActionDecl>());

            // ASSERT
            Assert.AreEqual(1, newValue);
        }

        [TestMethod]
        public void Can_GeneratehGoalCorrectly_MultipleGoals_2()
        {
            // ARRANGE
            var decl = new PDDLDecl(new DomainDecl(), new ProblemDecl());
            decl.Problem.Goal = new GoalDecl();
            decl.Problem.Goal.GoalExp = new AndExp(new List<IExp>() { new PredicateExp("goal-fact-1"), new PredicateExp("goal-fact-2"), new PredicateExp("goal-fact-3") });
            decl.Problem.Init = new InitDecl();
            decl.Problem.Init.Predicates.Add(new PredicateExp("goal-fact-1"));
            decl.Problem.Init.Predicates.Add(new PredicateExp("goal-fact-2"));
            decl.Problem.Init.Predicates.Add(new PredicateExp("goal-fact-3"));
            IHeuristic h = new hGoal(decl);
            var parent = new StateMove();
            var state = new PDDLStateSpace(decl);

            // ACT
            var newValue = h.GetValue(parent, state, new List<ActionDecl>());

            // ASSERT
            Assert.AreEqual(0, newValue);
        }
    }
}
