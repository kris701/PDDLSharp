using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.SAS;
using PDDLSharp.StateSpaces.SAS;
using PDDLSharp.Toolkit.Planners.Heuristics;
using PDDLSharp.Toolkit.Planners.Search;
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
            var decl = new SASDecl();
            decl.Goal.Clear();
            decl.Goal.Add(new Fact("goal-fact"));
            var h = new hGoal();
            var parent = new StateMove();
            var state = new SASStateSpace(decl);

            // ACT
            var newValue = h.GetValue(parent, state, new List<Operator>());

            // ASSERT
            Assert.AreEqual(1, newValue);
        }

        [TestMethod]
        public void Can_GeneratehGoalCorrectly_OneGoal()
        {
            // ARRANGE
            var decl = new SASDecl();
            decl.Goal.Clear();
            decl.Goal.Add(new Fact("goal-fact"));
            decl.Init.Clear();
            decl.Init.Add(new Fact("goal-fact"));
            var h = new hGoal();
            var parent = new StateMove();
            var state = new SASStateSpace(decl);

            // ACT
            var newValue = h.GetValue(parent, state, new List<Operator>());

            // ASSERT
            Assert.AreEqual(0, newValue);
        }

        [TestMethod]
        public void Can_GeneratehGoalCorrectly_MultipleGoals_1()
        {
            // ARRANGE
            var decl = new SASDecl();
            decl.Goal.Clear();
            decl.Goal.Add(new Fact("goal-fact-1"));
            decl.Goal.ElementAt(0).ID = 0;
            decl.Goal.Add(new Fact("goal-fact-2"));
            decl.Goal.ElementAt(1).ID = 1;
            decl.Goal.Add(new Fact("goal-fact-3"));
            decl.Goal.ElementAt(2).ID = 2;
            decl.Init.Clear();
            decl.Init.Add(new Fact("goal-fact-1"));
            decl.Init.ElementAt(0).ID = 0;
            decl.Init.Add(new Fact("goal-fact-2"));
            decl.Init.ElementAt(1).ID = 1;
            var h = new hGoal();
            var parent = new StateMove();
            var state = new SASStateSpace(decl);

            // ACT
            var newValue = h.GetValue(parent, state, new List<Operator>());

            // ASSERT
            Assert.AreEqual(1, newValue);
        }

        [TestMethod]
        public void Can_GeneratehGoalCorrectly_MultipleGoals_2()
        {
            // ARRANGE
            var decl = new SASDecl();
            decl.Goal.Clear();
            decl.Goal.Add(new Fact("goal-fact-1"));
            decl.Goal.Add(new Fact("goal-fact-2"));
            decl.Goal.Add(new Fact("goal-fact-3"));
            decl.Init.Clear();
            decl.Init.Add(new Fact("goal-fact-1"));
            decl.Init.Add(new Fact("goal-fact-2"));
            decl.Init.Add(new Fact("goal-fact-3"));
            var h = new hGoal();
            var parent = new StateMove();
            var state = new SASStateSpace(decl);

            // ACT
            var newValue = h.GetValue(parent, state, new List<Operator>());

            // ASSERT
            Assert.AreEqual(0, newValue);
        }
    }
}
