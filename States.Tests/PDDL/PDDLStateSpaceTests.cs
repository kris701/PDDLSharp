using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.States.PDDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.States.Tests.PDDL
{
    [TestClass]
    public class PDDLStateSpaceTests
    {
        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(10)]
        public void Can_GetCount(int addNodes)
        {
            // ARRANGE
            IPDDLState state = new PDDLStateSpace(new PDDLDecl(new DomainDecl(), new ProblemDecl()));
            for (int i = 0; i < addNodes; i++)
                state.Add(new PredicateExp($"something{i}"));

            // ACT

            // ASSERT
            Assert.AreEqual(addNodes, state.Count);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public void Can_Add_NoDuplicates_1(int addNodes)
        {
            // ARRANGE
            IPDDLState state = new PDDLStateSpace(new PDDLDecl(new DomainDecl(), new ProblemDecl()));

            // ACT
            for (int i = 0; i < addNodes; i++)
                state.Add(new PredicateExp("something"));

            // ASSERT
            Assert.AreEqual(1, state.Count);
            Assert.IsTrue(state.Contains(new PredicateExp("something")));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public void Can_Add_NoDuplicates_2(int addNodes)
        {
            // ARRANGE
            IPDDLState state = new PDDLStateSpace(new PDDLDecl(new DomainDecl(), new ProblemDecl()));

            // ACT
            for (int i = 0; i < addNodes; i++)
                state.Add("something");

            // ASSERT
            Assert.AreEqual(1, state.Count);
            Assert.IsTrue(state.Contains("something"));
        }

        [TestMethod]
        public void Can_Del_RemoveSame_1()
        {
            // ARRANGE
            IPDDLState state = new PDDLStateSpace(new PDDLDecl(new DomainDecl(), new ProblemDecl()));

            // ACT
            state.Add(new PredicateExp("something"));
            Assert.AreEqual(1, state.Count);
            state.Del(new PredicateExp("something"));

            // ASSERT
            Assert.AreEqual(0, state.Count);
            Assert.IsFalse(state.Contains(new PredicateExp("something")));
        }

        [TestMethod]
        public void Can_Del_RemoveSame_2()
        {
            // ARRANGE
            IPDDLState state = new PDDLStateSpace(new PDDLDecl(new DomainDecl(), new ProblemDecl()));

            // ACT
            state.Add("something");
            Assert.AreEqual(1, state.Count);
            state.Del("something");

            // ASSERT
            Assert.AreEqual(0, state.Count);
            Assert.IsFalse(state.Contains("something"));
        }

        [TestMethod]
        public void Cant_Del_IfNotThere_1()
        {
            // ARRANGE
            IPDDLState state = new PDDLStateSpace(new PDDLDecl(new DomainDecl(), new ProblemDecl()));

            // ACT
            Assert.AreEqual(0, state.Count);
            state.Del(new PredicateExp("something"));

            // ASSERT
            Assert.AreEqual(0, state.Count);
            Assert.IsFalse(state.Contains(new PredicateExp("something")));
        }

        [TestMethod]
        public void Cant_Del_IfNotThere_2()
        {
            // ARRANGE
            IPDDLState state = new PDDLStateSpace(new PDDLDecl(new DomainDecl(), new ProblemDecl()));

            // ACT
            Assert.AreEqual(0, state.Count);
            state.Del("something");

            // ASSERT
            Assert.AreEqual(0, state.Count);
            Assert.IsFalse(state.Contains("something"));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(10)]
        public void Can_Contain_IfThere_1(int addNodes)
        {
            // ARRANGE
            IPDDLState state = new PDDLStateSpace(new PDDLDecl(new DomainDecl(), new ProblemDecl()));
            for (int i = 0; i < addNodes; i++)
                state.Add(new PredicateExp($"something{i}"));

            // ACT

            // ASSERT
            for (int i = 0; i < addNodes; i++)
                Assert.IsTrue(state.Contains(new PredicateExp($"something{i}")));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(10)]
        public void Can_Contain_IfThere_2(int addNodes)
        {
            // ARRANGE
            IPDDLState state = new PDDLStateSpace(new PDDLDecl(new DomainDecl(), new ProblemDecl()));
            for (int i = 0; i < addNodes; i++)
                state.Add($"something{i}");

            // ACT

            // ASSERT
            for (int i = 0; i < addNodes; i++)
                Assert.IsTrue(state.Contains($"something{i}"));
        }
    }
}
