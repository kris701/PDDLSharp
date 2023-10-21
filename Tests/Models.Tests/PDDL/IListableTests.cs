using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.Tests.PDDL
{
    [TestClass]
    public class IListableTests
    {
        public static IEnumerable<object[]> GetTestData()
        {
            yield return new object[]
            {
                new AndExp(),
                new PredicateExp("pred")
            };
            yield return new object[]
            {
                new AndExp(new List<IExp>(){ new PredicateExp("pred2"), new PredicateExp("pred1") }),
                new PredicateExp("pred")
            };

            yield return new object[]
            {
                new OrExp(),
                new PredicateExp("pred")
            };
            yield return new object[]
            {
                new OrExp(new List<IExp>(){ new PredicateExp("pred2"), new PredicateExp("pred1") }),
                new PredicateExp("pred")
            };

            yield return new object[]
            {
                new ParameterExp(new List<NameExp>(){ new NameExp("?a"), new NameExp("?b") }),
                new NameExp("?c")
            };
            yield return new object[]
            {
                new ParameterExp(),
                new NameExp("?c")
            };

            yield return new object[]
            {
                new FunctionsDecl(),
                new PredicateExp("func3")
            };
            yield return new object[]
            {
                new FunctionsDecl(new List<PredicateExp>(){ new PredicateExp("func1"), new PredicateExp("func2") }),
                new PredicateExp("func3")
            };

            yield return new object[]
            {
                new TimelessDecl(),
                new PredicateExp("pred2")
            };
            yield return new object[]
{
                new TimelessDecl(new List<PredicateExp>(){ new PredicateExp("pred1") }),
                new PredicateExp("pred2")
};

            yield return new object[]
            {
                new TypesDecl(),
                new TypeExp("type3")
            };
            yield return new object[]
            {
                new TypesDecl(new List<TypeExp>(){ new TypeExp("type1"), new TypeExp("type2") }),
                new TypeExp("type3")
            };

            yield return new object[]
            {
                new ObjectsDecl(new List<NameExp>(){ new NameExp("obj1"), new NameExp("obj2") }),
                new NameExp("obj3")
            };
        }

        public static IEnumerable<object[]> GetRemoveAllTestData()
        {
            yield return new object[]
            {
                new AndExp(),
                new PredicateExp("pred")
            };

            yield return new object[]
            {
                new OrExp(),
                new PredicateExp("pred")
            };

            yield return new object[]
            {
                new ParameterExp(),
                new NameExp("?c")
            };

            yield return new object[]
            {
                new FunctionsDecl(),
                new PredicateExp("func3")
            };

            yield return new object[]
            {
                new TypesDecl(),
                new TypeExp("type3")
            };
        }

        public static IEnumerable<object[]> GetRangeTestData()
        {
            yield return new object[]
            {
                new AndExp(),
                new List<INode>(){ new PredicateExp("pred") , new PredicateExp("pred1") , new PredicateExp("pred2") }
            };

            yield return new object[]
            {
                new OrExp(),
                new List<INode>(){ new PredicateExp("pred") , new PredicateExp("pred1") , new PredicateExp("pred2") }
            };

            yield return new object[]
            {
                new ParameterExp(),
                new List<INode>(){ new NameExp("?a") , new NameExp("?b") , new NameExp("?c") }
            };

            yield return new object[]
            {
                new FunctionsDecl(),
                new List<INode>(){ new PredicateExp("func") , new PredicateExp("func1") , new PredicateExp("func2") }
            };

            yield return new object[]
            {
                new TypesDecl(),
                new List<INode>(){ new TypeExp("type") , new TypeExp("type1") , new TypeExp("type2") }
            };
        }

        // This is just a test with some random sets of nodes.
        // These tests are not exhaustive!
        [TestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void Can_Add(IListable node, INode add)
        {
            // ARRANGE
            Assert.IsFalse(node.Contains(add));

            // ACT

            node.Add(add);

            // ASSERT
            Assert.IsTrue(node.Contains(add));
        }

        [TestMethod]
        [DynamicData(nameof(GetRangeTestData), DynamicDataSourceType.Method)]
        public void Can_AddRange(IListable node, List<INode> adds)
        {
            // ARRANGE
            Assert.IsFalse(node.ContainsAll(adds));

            // ACT

            node.AddRange(adds);

            // ASSERT
            Assert.IsTrue(node.ContainsAll(adds));
        }

        // This is just a test with some random sets of nodes.
        // These tests are not exhaustive!
        [TestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void Can_Add_Duplicate(IListable node, INode add)
        {
            // ARRANGE
            Assert.IsFalse(node.Contains(add));

            // ACT
            node.Add(add);
            node.Add(add);

            // ASSERT
            Assert.AreEqual(2, node.Count(add));
        }

        // This is just a test with some random sets of nodes.
        // These tests are not exhaustive!
        [TestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void Can_Remove(IListable node, INode add)
        {
            // ARRANGE
            Assert.IsFalse(node.Contains(add));
            node.Add(add);
            Assert.IsTrue(node.Contains(add));

            // ACT
            node.Remove(add);

            // ASSERT
            Assert.IsFalse(node.Contains(add));
        }

        [TestMethod]
        [DynamicData(nameof(GetRangeTestData), DynamicDataSourceType.Method)]
        public void Can_RemoveRange(IListable node, List<INode> adds)
        {
            // ARRANGE
            Assert.IsFalse(node.ContainsAll(adds));
            node.AddRange(adds);
            Assert.IsTrue(node.ContainsAll(adds));

            // ACT
            node.RemoveRange(adds);

            // ASSERT
            Assert.IsFalse(node.ContainsAll(adds));
        }

        // This is just a test with some random sets of nodes.
        // These tests are not exhaustive!
        [TestMethod]
        [DynamicData(nameof(GetRemoveAllTestData), DynamicDataSourceType.Method)]
        public void Can_RemoveAll(IListable node, INode add)
        {
            // ARRANGE
            Assert.AreEqual(0, node.Count(add));
            for (int i = 0; i < 10; i++)
                node.Add(add);
            Assert.AreEqual(10, node.Count(add));

            // ACT
            node.RemoveAll(add);

            // ASSERT
            Assert.AreEqual(0, node.Count(add));
        }

        // This is just a test with some random sets of nodes.
        // These tests are not exhaustive!
        [TestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void Cant_Remove_IfNotThere(IListable node, INode add)
        {
            // ARRANGE
            Assert.IsFalse(node.Contains(add));

            // ACT
            node.Remove(add);

            // ASSERT
            Assert.IsFalse(node.Contains(add));
        }

        // This is just a test with some random sets of nodes.
        // These tests are not exhaustive!
        [TestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void Can_Replace(IListable node, INode with)
        {
            // ARRANGE
            INode target = null;
            foreach (var subnode in node)
            {
                if (subnode != with)
                {
                    target = subnode;
                    break;
                }
            }
            if (target == null)
                return;
            Assert.IsFalse(node.Contains(with));

            // ACT
            node.Replace(target, with);

            // ASSERT
            Assert.IsTrue(node.Contains(with));
            Assert.IsFalse(node.Contains(target));
        }
    }
}
