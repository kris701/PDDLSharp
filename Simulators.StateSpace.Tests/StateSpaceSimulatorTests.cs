using PDDLSharp.Models;
using PDDLSharp.Models.Domain;
using PDDLSharp.Models.Expressions;
using PDDLSharp.Models.Problem;
using PDDLSharp.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Simulators.StateSpace.Tests
{
    [TestClass]
    public class StateSpaceSimulatorTests
    {
        private PDDLDecl GetDecl(string domain, string problem)
        {
            IParser parser = new PDDLParser(null);
            return parser.Parse(domain, problem);
        }

        [TestMethod]
        public void Can_CopyState()
        {
            // ARRANGE
            var decl = GetDecl("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl");

            // ACT
            IStateSpaceSimulator simulator = new StateSpaceSimulator(decl);

            // ASSERT
            Assert.AreEqual(simulator.State.Count, decl.Problem.Init.Predicates.Count);
        }

        [TestMethod]
        public void Can_Step_Gripper_Move()
        {
            // ARRANGE
            var decl = GetDecl("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl");
            IStateSpaceSimulator simulator = new StateSpaceSimulator(decl);

            // ACT
            simulator.Step("move", new string[] { "rooma", "roomb" });

            // ASSERT
            Assert.IsTrue(simulator.State.Contains(new Operator("at-robby", new string[] { "roomb" })));
            Assert.IsFalse(simulator.State.Contains(new Operator("at-robby", new string[] { "rooma" })));
        }

        [TestMethod]
        public void Can_Step_Gripper_Move_Move()
        {
            // ARRANGE
            var decl = GetDecl("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl");
            IStateSpaceSimulator simulator = new StateSpaceSimulator(decl);

            // ACT
            simulator.Step("move", new string[] { "rooma", "roomb" });
            simulator.Step("move", new string[] { "roomb", "rooma" });

            // ASSERT
            Assert.IsTrue(simulator.State.Contains(new Operator("at-robby", new string[] { "rooma" })));
            Assert.IsFalse(simulator.State.Contains(new Operator("at-robby", new string[] { "roomb" })));
        }

        [TestMethod]
        public void Can_Step_Gripper_Pick_Move_Drop()
        {
            // ARRANGE
            var decl = GetDecl("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl");
            IStateSpaceSimulator simulator = new StateSpaceSimulator(decl);

            // ACT
            simulator.Step("pick", new string[] { "ball1", "rooma", "left" });
            simulator.Step("move", new string[] { "rooma", "roomb" });
            simulator.Step("drop", new string[] { "ball1", "roomb", "left" });

            // ASSERT
            Assert.IsTrue(simulator.State.Contains(new Operator("at", new string[] { "ball1", "roomb" })));
            Assert.IsFalse(simulator.State.Contains(new Operator("at", new string[] { "ball1", "rooma" })));
        }
    }
}
