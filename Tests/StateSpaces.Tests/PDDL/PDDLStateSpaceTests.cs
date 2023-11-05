using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.StateSpaces.PDDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.StateSpaces.Tests
{
    [TestClass]
    public class PDDLStateSpaceTests
    {
        #region Basics 

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(10)]
        public void Can_GetCount(int addNodes)
        {
            // ARRANGE
            PDDLStateSpace state = new PDDLStateSpace(new PDDLDecl(new DomainDecl(), new ProblemDecl()));
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
            PDDLStateSpace state = new PDDLStateSpace(new PDDLDecl(new DomainDecl(), new ProblemDecl()));

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
            PDDLStateSpace state = new PDDLStateSpace(new PDDLDecl(new DomainDecl(), new ProblemDecl()));

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
            PDDLStateSpace state = new PDDLStateSpace(new PDDLDecl(new DomainDecl(), new ProblemDecl()));

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
            PDDLStateSpace state = new PDDLStateSpace(new PDDLDecl(new DomainDecl(), new ProblemDecl()));

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
            PDDLStateSpace state = new PDDLStateSpace(new PDDLDecl(new DomainDecl(), new ProblemDecl()));

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
            PDDLStateSpace state = new PDDLStateSpace(new PDDLDecl(new DomainDecl(), new ProblemDecl()));

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
            PDDLStateSpace state = new PDDLStateSpace(new PDDLDecl(new DomainDecl(), new ProblemDecl()));
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
            PDDLStateSpace state = new PDDLStateSpace(new PDDLDecl(new DomainDecl(), new ProblemDecl()));
            for (int i = 0; i < addNodes; i++)
                state.Add($"something{i}");

            // ACT

            // ASSERT
            for (int i = 0; i < addNodes; i++)
                Assert.IsTrue(state.Contains($"something{i}"));
        }

        #endregion

        #region ExecuteNode

        public static IEnumerable<object[]> GetExecuteNodeData()
        {
            yield return new object[] {
                new PredicateExp("a"),
                1
            };

            yield return new object[] {
                new NotExp(new PredicateExp("a")),
                0
            };

            yield return new object[] {
                new NotExp(new NotExp(new PredicateExp("a"))),
                1
            };

            yield return new object[] {
                new AndExp(new List<IExp>()
                {
                    new PredicateExp("a"),
                    new PredicateExp("b"),
                    new PredicateExp("c"),
                }),
                3
            };

            yield return new object[] {
                new AndExp(new List<IExp>()
                {
                    new PredicateExp("a"),
                    new PredicateExp("a"),
                    new PredicateExp("a"),
                }),
                1
            };

            yield return new object[] {
                new WhenExp(new PredicateExp("a"), new PredicateExp("b")),
                0
            };

            yield return new object[] {
                new WhenExp(new NotExp(new PredicateExp("a")), new PredicateExp("b")),
                1
            };

            yield return new object[] {
                new AndExp(new List<IExp>()
                {
                    new PredicateExp("a"),
                    new WhenExp(new PredicateExp("a"), new PredicateExp("b")),
                }),
                1
            };

            yield return new object[] {
                new AndExp(new List<IExp>()
                {
                    new WhenExp(new PredicateExp("a"), new PredicateExp("b")),
                    new PredicateExp("a"),
                }),
                1
            };

            yield return new object[] {
                new ForAllExp(
                    new ParameterExp(new List<NameExp>(){ new NameExp("?a") }),
                    new PredicateExp("pred", new List<NameExp>(){new NameExp("?a") })),
                2
            };

            yield return new object[] {
                new ForAllExp(
                    new ParameterExp(new List<NameExp>(){ new NameExp("?a"), new NameExp("?b") }),
                    new ForAllExp(
                        new ParameterExp(new List<NameExp>(){ new NameExp("?c"), new NameExp("?d") }),
                        new PredicateExp("pred", new List<NameExp>(){new NameExp("?a"), new NameExp("?b"), new NameExp("?c"), new NameExp("?d") })
                    )
                ),
                16
            };
        }

        [TestMethod]
        [DynamicData(nameof(GetExecuteNodeData), DynamicDataSourceType.Method)]
        public void Can_ExecuteNode_ExpectedNodes(INode node, int expected)
        {
            // ARRANGE
            var decl = new PDDLDecl(new DomainDecl(), new ProblemDecl());
            decl.Problem.Objects = new ObjectsDecl(new List<NameExp>() { new NameExp("obja"), new NameExp("objb") });
            PDDLStateSpace state = new PDDLStateSpace(decl);

            // ACT
            state.ExecuteNode(node);

            // ASSERT
            Assert.AreEqual(expected, state.Count);
        }

        #endregion

        #region IsNodeTrue

        public static IEnumerable<object[]> GetIsNodeTrueData()
        {
            yield return new object[] {
                new PredicateExp("a"),
                new PredicateExp("a"),
                true
            };

            yield return new object[] {
                new PredicateExp("b"),
                new PredicateExp("a"),
                false
            };

            yield return new object[] {
                new PredicateExp("b"),
                new NotExp(new PredicateExp("a")),
                true
            };

            yield return new object[] {
                new PredicateExp("b"),
                new NotExp(new NotExp(new PredicateExp("a"))),
                false
            };

            yield return new object[] {
                new AndExp(new List<IExp>(){
                    new PredicateExp("a"),
                    new PredicateExp("b"),
                }),
                new WhenExp(new PredicateExp("a"), new PredicateExp("b")),
                true
            };

            yield return new object[] {
                new AndExp(new List<IExp>(){
                    new PredicateExp("a"),
                    new PredicateExp("b"),
                }),
                new WhenExp(new PredicateExp("a"), new PredicateExp("c")),
                false
            };

            yield return new object[] {
                new AndExp(new List<IExp>(){
                    new PredicateExp("a"),
                    new PredicateExp("b"),
                }),
                new ImplyExp(new PredicateExp("c"), new PredicateExp("b")),
                true
            };

            yield return new object[] {
                new AndExp(new List<IExp>(){
                    new PredicateExp("a"),
                    new PredicateExp("b"),
                }),
                new ImplyExp(new PredicateExp("a"), new PredicateExp("b")),
                true
            };

            yield return new object[] {
                new AndExp(new List<IExp>(){
                    new PredicateExp("a"),
                    new PredicateExp("b"),
                }),
                new ImplyExp(new PredicateExp("a"), new PredicateExp("c")),
                false
            };

            yield return new object[] {
                new ForAllExp(
                    new ParameterExp(new List<NameExp>(){ new NameExp("?a") }),
                    new PredicateExp("pred", new List<NameExp>(){ new NameExp("?a") })),
                new AndExp(new List<IExp>(){
                    new PredicateExp("pred", new List<NameExp>(){ new NameExp("obja") }),
                    new PredicateExp("pred", new List<NameExp>(){ new NameExp("objb") }),
                }),
                true
            };

            yield return new object[] {
                new AndExp(new List<IExp>(){
                    new PredicateExp("pred", new List<NameExp>(){ new NameExp("obja") }),
                    new PredicateExp("pred", new List<NameExp>(){ new NameExp("objb") }),
                }),
                new ForAllExp(
                    new ParameterExp(new List<NameExp>(){ new NameExp("?a") }),
                    new PredicateExp("pred", new List<NameExp>(){ new NameExp("?a") })),
                true
            };

            yield return new object[] {
                new AndExp(new List<IExp>(){
                    new PredicateExp("pred", new List<NameExp>(){ new NameExp("obja") }),
                    new PredicateExp("pred", new List<NameExp>(){ new NameExp("objb") }),
                }),
                new ForAllExp(
                    new ParameterExp(new List<NameExp>(){ new NameExp("?a"), new NameExp("?b") }),
                    new PredicateExp("pred", new List<NameExp>(){ new NameExp("?a"), new NameExp("?b") })),
                false
            };

            yield return new object[] {
                new AndExp(new List<IExp>(){
                    new PredicateExp("pred", new List<NameExp>(){ new NameExp("obja") }),
                }),
                new ExistsExp(
                    new ParameterExp(new List<NameExp>(){ new NameExp("?a") }),
                    new PredicateExp("pred", new List<NameExp>(){ new NameExp("?a") })),
                true
            };

            yield return new object[] {
                new AndExp(new List<IExp>(){
                    new PredicateExp("pred", new List<NameExp>(){ new NameExp("objb") }),
                }),
                new ExistsExp(
                    new ParameterExp(new List<NameExp>(){ new NameExp("?a") }),
                    new PredicateExp("pred", new List<NameExp>(){ new NameExp("?a") })),
                true
            };

            yield return new object[] {
                new AndExp(new List<IExp>(){
                    new PredicateExp("pred", new List<NameExp>(){ new NameExp("obja") }),
                }),
                new ExistsExp(
                    new ParameterExp(new List<NameExp>(){ new NameExp("?a"), new NameExp("?b") }),
                    new PredicateExp("pred", new List<NameExp>(){ new NameExp("?a"), new NameExp("?b") })),
                false
            };

            yield return new object[] {
                new AndExp(new List<IExp>(){
                    new PredicateExp("a"),
                }),
                new OrExp(new List<IExp>()
                {
                    new PredicateExp("a"),
                    new PredicateExp("b"),
                }),
                true
            };

            yield return new object[] {
                new AndExp(new List<IExp>(){
                    new PredicateExp("b"),
                }),
                new OrExp(new List<IExp>()
                {
                    new PredicateExp("a"),
                    new PredicateExp("b"),
                }),
                true
            };

            yield return new object[] {
                new AndExp(new List<IExp>(){
                }),
                new OrExp(new List<IExp>()
                {
                    new PredicateExp("a"),
                    new PredicateExp("b"),
                }),
                false
            };
        }

        [TestMethod]
        [DynamicData(nameof(GetIsNodeTrueData), DynamicDataSourceType.Method)]
        public void Can_IsNodeTrue(INode apply, INode check, bool expected)
        {
            // ARRANGE
            var decl = new PDDLDecl(new DomainDecl(), new ProblemDecl());
            decl.Problem.Objects = new ObjectsDecl(new List<NameExp>() { new NameExp("obja"), new NameExp("objb") });
            PDDLStateSpace state = new PDDLStateSpace(decl);
            state.ExecuteNode(apply);

            // ACT
            // ASSERT
            Assert.AreEqual(expected, state.IsNodeTrue(check));
        }

        #endregion
    }
}
