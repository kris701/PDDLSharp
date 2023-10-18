using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.Tests.PDDL
{
    [TestClass]
    public class IWalkableTests
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
                new FunctionsDecl(new List<PredicateExp>(){ new PredicateExp("func1"), new PredicateExp("func2") }),
                new PredicateExp("func3")
            };
        }

        // This is just a test with some random sets of nodes.
        // These tests are not exhaustive!
        [TestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void Can_Add(IWalkable node, INode add)
        {
            // ARRANGE
            // ACT
            foreach (var child in node)
                Assert.AreNotEqual(child, add);

            node.Add(add);

            // ASSERT
            int howMany = 0;
            foreach (var child in node)
                if (child == add)
                    howMany++;
            Assert.AreEqual(1, howMany);
        }

        // This is just a test with some random sets of nodes.
        // These tests are not exhaustive!
        [TestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void Can_Add_Duplicate(IWalkable node, INode add)
        {
            // ARRANGE
            // ACT
            foreach (var child in node)
                Assert.AreNotEqual(child, add);

            node.Add(add);
            node.Add(add);

            // ASSERT
            int howMany = 0;
            foreach (var child in node)
                if (child == add)
                    howMany++;
            Assert.AreEqual(2, howMany);
        }

        // This is just a test with some random sets of nodes.
        // These tests are not exhaustive!
        [TestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void Can_Remove(IWalkable node, INode add)
        {
            // ARRANGE
            node.Add(add);
            int howMany = 0;
            foreach (var child in node)
                if (child == add)
                    howMany++;
            Assert.AreEqual(1, howMany);

            // ACT

            node.Remove(add);

            // ASSERT
            foreach (var child in node)
                Assert.AreNotEqual(child, add);
        }

        // This is just a test with some random sets of nodes.
        // These tests are not exhaustive!
        [TestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void Cant_Remove_IfNotThere(IWalkable node, INode add)
        {
            // ARRANGE
            foreach (var child in node)
                Assert.AreNotEqual(child, add);

            // ACT

            node.Remove(add);

            // ASSERT
            foreach (var child in node)
                Assert.AreNotEqual(child, add);
        }

        // This is just a test with some random sets of nodes.
        // These tests are not exhaustive!
        [TestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void Can_Replace(IWalkable node, INode with)
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

            foreach (var child in node)
                Assert.AreNotEqual(child, with);

            // ACT

            node.Replace(target, with);

            // ASSERT
            int howMany = 0;
            foreach (var child in node)
                if (child == with)
                    howMany++;
            Assert.AreEqual(1, howMany);
            howMany = 0;
            foreach (var child in node)
                if (child == target)
                    howMany++;
            Assert.AreEqual(0, howMany);
        }
    }
}
