using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.Plans;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.PDDL;
using PDDLSharp.Toolkit.StateSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.PlanValidator.Tests
{
    [TestClass]
    public class PlanValidatorTests
    {
        private PDDLDecl GetDecl(string domain, string problem)
        {
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);
            PDDLParser parser = new PDDLParser(listener);
            return parser.ParseDecl(new FileInfo(domain), new FileInfo(problem));
        }

        [TestMethod]
        public void Can_ExecutePlan_Gripper_Move_Move()
        {
            // ARRANGE
            var decl = GetDecl("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl");
            decl.Problem.Goal.GoalExp = new PredicateExp("at-robby", new List<NameExp>() { new NameExp("rooma") });
            IPlanValidator validator = new PlanValidator();
            var newPlan = new ActionPlan(new List<GroundedAction>(), 2);
            newPlan.Plan.Add(new GroundedAction("move", "rooma", "roomb"));
            newPlan.Plan.Add(new GroundedAction("move", "roomb", "rooma"));

            // ACT
            Assert.IsTrue(validator.Validate(newPlan, decl));
        }

        [TestMethod]
        public void Cant_ExecutePlan_Gripper_Move_Move_IfWrong()
        {
            // ARRANGE
            var decl = GetDecl("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl");
            IPlanValidator validator = new PlanValidator();
            var newPlan = new ActionPlan(new List<GroundedAction>(), 2);
            newPlan.Plan.Add(new GroundedAction("move", "rooma", "roomb"));
            newPlan.Plan.Add(new GroundedAction("move", "roomb", "rooma"));
            newPlan.Plan.Add(new GroundedAction("move", "roomb", "rooma"));

            // ACT
            Assert.IsFalse(validator.Validate(newPlan, decl));
        }

        [TestMethod]
        public void Can_ExecutePlan_Gripper_Pick_Move_Drop()
        {
            // ARRANGE
            var decl = GetDecl("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl");
            decl.Problem.Goal.GoalExp = new PredicateExp("at", new List<NameExp>() { new NameExp("ball1"), new NameExp("roomb") });
            IPlanValidator validator = new PlanValidator();
            var newPlan = new ActionPlan(new List<GroundedAction>(), 3);
            newPlan.Plan.Add(new GroundedAction("pick", "ball1", "rooma", "left"));
            newPlan.Plan.Add(new GroundedAction("move", "rooma", "roomb"));
            newPlan.Plan.Add(new GroundedAction("drop", "ball1", "roomb", "left"));

            // ACT
            Assert.IsTrue(validator.Validate(newPlan, decl));
        }

        [TestMethod]
        public void Cant_ExecutePlan_Gripper_Pick_Move_Drop_IfWrong()
        {
            // ARRANGE
            var decl = GetDecl("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl");
            IPlanValidator validator = new PlanValidator();
            var newPlan = new ActionPlan(new List<GroundedAction>(), 3);
            newPlan.Plan.Add(new GroundedAction("pick", "ball1", "rooma", "left"));
            newPlan.Plan.Add(new GroundedAction("move", "rooma", "roomb"));
            newPlan.Plan.Add(new GroundedAction("drop", "ball1", "roomb", "left"));
            newPlan.Plan.Add(new GroundedAction("drop", "ball1", "roomb", "left"));

            // ACT
            Assert.IsFalse(validator.Validate(newPlan, decl));
        }
    }
}
