using PDDLSharp.Analysers.SAS;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.SAS;
using PDDLSharp.Parsers.PDDL;
using PDDLSharp.Translators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Analysers.Tests.SAS
{
    [TestClass]
    public class SASAnalyserTests
    {
        #region CheckForBasicSASDecl

        [TestMethod]
        public void Can_CheckForBasicSASDecl()
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            var analyser = new SASAnalyser(listener);

            var decl = new SASDecl();
            decl.DomainVariables = new HashSet<string>() { "a" };
            decl.Operators = new List<Operator>() { new Operator() };
            decl.Init = new HashSet<Fact>() { new Fact("b") };
            decl.Goal = new HashSet<Fact>() { new Fact("b") };

            // ACT
            analyser.CheckForBasicSAS(decl);

            // ASSERT
            Assert.AreEqual(0, listener.Errors.Count);
        }

        [TestMethod]
        public void Can_CheckForBasicSASDecl_NoDomainVars()
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            var analyser = new SASAnalyser(listener);

            var decl = new SASDecl();
            decl.Operators = new List<Operator>() { new Operator() };
            decl.Init = new HashSet<Fact>() { new Fact("b") };
            decl.Goal = new HashSet<Fact>() { new Fact("b") };

            // ACT
            analyser.CheckForBasicSAS(decl);

            // ASSERT
            Assert.AreEqual(1, listener.Errors.Count);
        }

        [TestMethod]
        public void Can_CheckForBasicSASDecl_NoOperators()
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            var analyser = new SASAnalyser(listener);

            var decl = new SASDecl();
            decl.DomainVariables = new HashSet<string>() { "a" };
            decl.Init = new HashSet<Fact>() { new Fact("b") };
            decl.Goal = new HashSet<Fact>() { new Fact("b") };

            // ACT
            analyser.CheckForBasicSAS(decl);

            // ASSERT
            Assert.AreEqual(1, listener.Errors.Count);
        }

        [TestMethod]
        public void Can_CheckForBasicSASDecl_NoInits()
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            var analyser = new SASAnalyser(listener);

            var decl = new SASDecl();
            decl.DomainVariables = new HashSet<string>() { "a" };
            decl.Operators = new List<Operator>() { new Operator() };
            decl.Goal = new HashSet<Fact>() { new Fact("b") };

            // ACT
            analyser.CheckForBasicSAS(decl);

            // ASSERT
            Assert.AreEqual(1, listener.Errors.Count);
        }

        [TestMethod]
        public void Can_CheckForBasicSASDecl_NoGoals()
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            var analyser = new SASAnalyser(listener);

            var decl = new SASDecl();
            decl.DomainVariables = new HashSet<string>() { "a" };
            decl.Operators = new List<Operator>() { new Operator() };
            decl.Init = new HashSet<Fact>() { new Fact("b") };

            // ACT
            analyser.CheckForBasicSAS(decl);

            // ASSERT
            Assert.AreEqual(1, listener.Errors.Count);
        }

        #endregion

        #region InitReachabilityCheck

        [TestMethod]
        public void Can_InitReachabilityCheck_Valid_1()
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            var analyser = new SASAnalyser(listener);

            var decl = new SASDecl();
            decl.Operators = new List<Operator>() { new Operator("op", new string[0], new Fact[1] { new Fact("b") }, new Fact[0], new Fact[0]) };
            decl.Init = new HashSet<Fact>() { new Fact("b") };

            // ACT
            analyser.InitReachabilityCheck(decl);

            // ASSERT
            Assert.AreEqual(0, listener.Errors.Count);
        }

        [TestMethod]
        public void Can_InitReachabilityCheck_Valid_2()
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            var analyser = new SASAnalyser(listener);

            var decl = new SASDecl();
            decl.Operators = new List<Operator>() { 
                new Operator("op1", new string[0], new Fact[1] { new Fact("b") }, new Fact[0], new Fact[0]),
                new Operator("op2", new string[0], new Fact[1] { new Fact("q") }, new Fact[0], new Fact[0])
        };
            decl.Init = new HashSet<Fact>() { new Fact("b") };

            // ACT
            analyser.InitReachabilityCheck(decl);

            // ASSERT
            Assert.AreEqual(0, listener.Errors.Count);
        }

        [TestMethod]
        public void Can_InitReachabilityCheck_InValid()
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            var analyser = new SASAnalyser(listener);

            var decl = new SASDecl();
            decl.Operators = new List<Operator>() { new Operator("op", new string[0], new Fact[1] { new Fact("q") }, new Fact[0], new Fact[0]) };
            decl.Init = new HashSet<Fact>() { new Fact("b") };

            // ACT
            analyser.InitReachabilityCheck(decl);

            // ASSERT
            Assert.AreEqual(1, listener.Errors.Count);
        }

        #endregion

        #region GoalReachabilityCheck

        [TestMethod]
        public void Can_GoalReachabilityCheck_Valid_1()
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            var analyser = new SASAnalyser(listener);

            var decl = new SASDecl();
            decl.Operators = new List<Operator>() { new Operator("op", new string[0], new Fact[0], new Fact[1] { new Fact("b") }, new Fact[0]) };
            decl.Goal = new HashSet<Fact>() { new Fact("b") };

            // ACT
            analyser.GoalReachabilityCheck(decl);

            // ASSERT
            Assert.AreEqual(0, listener.Errors.Count);
        }

        [TestMethod]
        public void Can_GoalReachabilityCheck_Valid_2()
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            var analyser = new SASAnalyser(listener);

            var decl = new SASDecl();
            decl.Operators = new List<Operator>() { 
                new Operator("op", new string[0], new Fact[0], new Fact[1] { new Fact("b") }, new Fact[0]),
                new Operator("op", new string[0], new Fact[0], new Fact[1] { new Fact("q") }, new Fact[0])
            };
            decl.Goal = new HashSet<Fact>() { new Fact("b") };

            // ACT
            analyser.GoalReachabilityCheck(decl);

            // ASSERT
            Assert.AreEqual(0, listener.Errors.Count);
        }

        [TestMethod]
        public void Can_GoalReachabilityCheck_Valid_3()
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            var analyser = new SASAnalyser(listener);

            var decl = new SASDecl();
            decl.Operators = new List<Operator>() {
                new Operator("op1", new string[0], new Fact[0], new Fact[1] { new Fact("b") }, new Fact[0]),
                new Operator("op2", new string[0], new Fact[0], new Fact[1] { new Fact("q") }, new Fact[0])
            };
            decl.Goal = new HashSet<Fact>() { new Fact("b"), new Fact("q") };

            // ACT
            analyser.GoalReachabilityCheck(decl);

            // ASSERT
            Assert.AreEqual(0, listener.Errors.Count);
        }

        [TestMethod]
        public void Can_GoalReachabilityCheck_InValid_1()
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            var analyser = new SASAnalyser(listener);

            var decl = new SASDecl();
            decl.Operators = new List<Operator>() {
                new Operator("op", new string[0], new Fact[0], new Fact[1] { new Fact("b") }, new Fact[0])
            };
            decl.Goal = new HashSet<Fact>() { new Fact("q") };

            // ACT
            analyser.GoalReachabilityCheck(decl);

            // ASSERT
            Assert.AreEqual(0, listener.Errors.Count);
        }

        [TestMethod]
        public void Can_GoalReachabilityCheck_InValid_2()
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            var analyser = new SASAnalyser(listener);

            var decl = new SASDecl();
            decl.Operators = new List<Operator>() {
                new Operator("op1", new string[0], new Fact[0], new Fact[1] { new Fact("b") }, new Fact[0]),
                new Operator("op2", new string[0], new Fact[0], new Fact[1] { new Fact("c") }, new Fact[0]),
                new Operator("op3", new string[0], new Fact[0], new Fact[1] { new Fact("a") }, new Fact[0]),
                new Operator("op4", new string[0], new Fact[0], new Fact[1] { new Fact("f") }, new Fact[0]),
            };
            decl.Goal = new HashSet<Fact>() { new Fact("q") };

            // ACT
            analyser.GoalReachabilityCheck(decl);

            // ASSERT
            Assert.AreEqual(0, listener.Errors.Count);
        }

        #endregion

        #region RelaxedReachabilityCheck

        [TestMethod]
        [DataRow("SAS/TestFiles/gripper-domain.pddl", "SAS/TestFiles/gripper-prob01.pddl")]
        [DataRow("SAS/TestFiles/satellite-domain.pddl", "SAS/TestFiles/satellite-prob01.pddl")]
        public void Can_RelaxedReachabilityCheck_Valid(string domain, string problem)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            var parser = new PDDLParser(listener);
            var pddlDecl = parser.ParseDecl(new FileInfo(domain), new FileInfo(problem));
            var translator = new PDDLToSASTranslator();
            var decl = translator.Translate(pddlDecl);
            var analyser = new SASAnalyser(listener);
            listener.Errors.Clear();

            // ACT
            analyser.RelaxedReachabilityCheck(decl);

            // ASSERT
            Assert.AreEqual(0, listener.Errors.Count);
        }

        [TestMethod]
        [DataRow("SAS/TestFiles/gripper-domain.pddl", "SAS/TestFiles/gripper-prob01.pddl")]
        [DataRow("SAS/TestFiles/satellite-domain.pddl", "SAS/TestFiles/satellite-prob01.pddl")]
        public void Can_RelaxedReachabilityCheck_InValid(string domain, string problem)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            var parser = new PDDLParser(listener);
            var pddlDecl = parser.ParseDecl(new FileInfo(domain), new FileInfo(problem));
            var translator = new PDDLToSASTranslator();
            var decl = translator.Translate(pddlDecl);
            var analyser = new SASAnalyser(listener);
            listener.Errors.Clear();

            decl.Goal.Add(new Fact("non-existentfact"));

            // ACT
            analyser.RelaxedReachabilityCheck(decl);

            // ASSERT
            Assert.AreEqual(1, listener.Errors.Count);
        }

        #endregion
    }
}
