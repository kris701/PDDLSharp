using PDDLSharp.ErrorListeners;
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
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);
            PDDLParser parser = new PDDLParser(listener);
            return parser.ParseDecl(domain, problem);
        }

        #region Gripper

        [TestMethod]
        public void Can_CopyState_Gripper()
        {
            // ARRANGE
            var decl = GetDecl("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl");

            // ACT
            IStateSpaceSimulator simulator = new StateSpaceSimulator(decl);

            // ASSERT
            Assert.AreEqual(simulator.State.Count, decl.Problem.Init.Predicates.Count);
        }

        [TestMethod]
        public void Can_Reset()
        {
            // ARRANGE
            var decl = GetDecl("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl");
            IStateSpaceSimulator simulator = new StateSpaceSimulator(decl);

            // ACT
            Assert.IsFalse(simulator.Contains("at-robby", "roomb"));
            Assert.IsTrue(simulator.Contains("at-robby", "rooma"));
            simulator.Step("move", "rooma", "roomb");
            Assert.IsTrue(simulator.Contains("at-robby", "roomb"));
            Assert.IsFalse(simulator.Contains("at-robby", "rooma"));
            simulator.Reset();

            // ASSERT
            Assert.IsFalse(simulator.Contains("at-robby", "roomb"));
            Assert.IsTrue(simulator.Contains("at-robby", "rooma"));
        }

        [TestMethod]
        public void Can_Step_Gripper_Move()
        {
            // ARRANGE
            var decl = GetDecl("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl");
            IStateSpaceSimulator simulator = new StateSpaceSimulator(decl);

            // ACT
            simulator.Step("move", "rooma", "roomb");

            // ASSERT
            Assert.IsTrue(simulator.Contains("at-robby", "roomb"));
            Assert.IsFalse(simulator.Contains("at-robby", "rooma"));
        }

        [TestMethod]
        public void Can_Step_Gripper_Move_Cost()
        {
            // ARRANGE
            var decl = GetDecl("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl");
            IStateSpaceSimulator simulator = new StateSpaceSimulator(decl);

            // ACT
            simulator.Step("move", "rooma", "roomb");

            // ASSERT
            Assert.AreEqual(1, simulator.Cost);
        }

        [TestMethod]
        public void Can_Step_Gripper_Move_Move()
        {
            // ARRANGE
            var decl = GetDecl("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl");
            IStateSpaceSimulator simulator = new StateSpaceSimulator(decl);

            // ACT
            simulator.Step("move", "rooma", "roomb");
            simulator.Step("move", "roomb", "rooma");

            // ASSERT
            Assert.IsTrue(simulator.Contains("at-robby", "rooma"));
            Assert.IsFalse(simulator.Contains("at-robby", "roomb"));
        }

        [TestMethod]
        public void Can_Step_Gripper_Move_Move_Cost()
        {
            // ARRANGE
            var decl = GetDecl("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl");
            IStateSpaceSimulator simulator = new StateSpaceSimulator(decl);

            // ACT
            simulator.Step("move", "rooma", "roomb");
            simulator.Step("move", "roomb", "rooma");

            // ASSERT
            Assert.AreEqual(2, simulator.Cost);
        }

        [TestMethod]
        public void Can_Step_Gripper_Pick_Move_Drop()
        {
            // ARRANGE
            var decl = GetDecl("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl");
            IStateSpaceSimulator simulator = new StateSpaceSimulator(decl);

            // ACT
            simulator.Step("pick", "ball1", "rooma", "left");
            simulator.Step("move", "rooma", "roomb");
            simulator.Step("drop", "ball1", "roomb", "left" );

            // ASSERT
            Assert.IsTrue(simulator.Contains("at", "ball1", "roomb"));
            Assert.IsFalse(simulator.Contains("at", "ball1", "rooma"));
        }

        [TestMethod]
        public void Can_Step_Gripper_Pick_Move_Drop_Cost()
        {
            // ARRANGE
            var decl = GetDecl("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl");
            IStateSpaceSimulator simulator = new StateSpaceSimulator(decl);

            // ACT
            simulator.Step("pick", "ball1", "rooma", "left");
            simulator.Step("move", "rooma", "roomb");
            simulator.Step("drop", "ball1", "roomb", "left");

            // ASSERT
            Assert.AreEqual(3, simulator.Cost);
        }

        #endregion

        #region Satellite

        [TestMethod]
        public void Can_CopyState_Satellite()
        {
            // ARRANGE
            var decl = GetDecl("TestFiles/satellite-domain.pddl", "TestFiles/satellite-prob01.pddl");

            // ACT
            IStateSpaceSimulator simulator = new StateSpaceSimulator(decl);

            // ASSERT
            Assert.AreEqual(simulator.State.Count, decl.Problem.Init.Predicates.Count);
        }

        [TestMethod]
        public void Can_Step_Satellite_TurnTo()
        {
            // ARRANGE
            var decl = GetDecl("TestFiles/satellite-domain.pddl", "TestFiles/satellite-prob01.pddl");
            IStateSpaceSimulator simulator = new StateSpaceSimulator(decl);

            // ACT
            simulator.Step("turn_to", "satellite0", "groundStation2", "phenomenon6");

            // ASSERT
            Assert.IsTrue(simulator.Contains("pointing", "satellite0", "groundstation2"));
            Assert.IsFalse(simulator.Contains("pointing", "satellite0", "phenomenon6"));
        }

        #endregion
    }
}
