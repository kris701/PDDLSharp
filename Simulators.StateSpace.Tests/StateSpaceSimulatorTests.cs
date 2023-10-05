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
            IParser parser = new PDDLParser(listener);
            return parser.Parse(domain, problem);
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
            simulator.Step("turn_to", new string[] { "satellite0", "groundStation2", "phenomenon6" });

            // ASSERT
            Assert.IsTrue(simulator.State.Contains(new Operator("pointing", new string[] { "satellite0", "groundstation2" })));
            Assert.IsFalse(simulator.State.Contains(new Operator("pointing", new string[] { "satellite0", "phenomenon6" })));
        }

        #endregion

        #region Satellite Stackelberg

        [TestMethod]
        public void Can_Step_SatelliteStakelberg_PassTurn()
        {
            // ARRANGE
            var decl = GetDecl("TestFiles/satellite-stackelberg-domain.pddl", "TestFiles/satellite-stackelberg-prob01.pddl");
            IStateSpaceSimulator simulator = new StateSpaceSimulator(decl);

            // ACT
            Assert.AreEqual(28, simulator.State.Count);
            simulator.Step("fix_pass-turn");

            // ASSERT
            Assert.AreEqual(47, simulator.State.Count);
        }

        [TestMethod]
        public void Can_Step_SatelliteStakelberg_Test()
        {
            // ARRANGE
            var decl = GetDecl("TestFiles/satellite-stackelberg-domain.pddl", "TestFiles/satellite-stackelberg-prob01.pddl");
            IStateSpaceSimulator simulator = new StateSpaceSimulator(decl);

            // ACT
            simulator.Step("fix_pick", new string[] { "ball4", "rooma", "left" });
            simulator.Step("fix_pick", new string[] { "ball3", "rooma", "right" });
            simulator.Step("fix_move", new string[] { "rooma", "roomb" });
            simulator.Step("fix_drop", new string[] { "ball4", "roomb", "left" });
            simulator.Step("fix_move", new string[] { "roomb", "rooma" });
            simulator.Step("fix_pick", new string[] { "ball2", "rooma", "left" });
            simulator.Step("fix_pass-turn");

            simulator.Step("attack_pick", new string[] { "ball4", "rooma", "left" });
            simulator.Step("attack_pick_goal", new string[] { "ball3", "rooma", "right" });
            simulator.Step("attack_move", new string[] { "rooma", "roomb" });
            simulator.Step("attack_drop_goal", new string[] { "ball4", "roomb", "left" });
            simulator.Step("attack_move_goal", new string[] { "roomb", "rooma" });
            simulator.Step("attack_pick_goal", new string[] { "ball2", "rooma", "left" });

            // ASSERT
        }

        #endregion
    }
}
