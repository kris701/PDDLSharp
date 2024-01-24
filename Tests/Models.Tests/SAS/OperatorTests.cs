using PDDLSharp.Models.SAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.Tests.SAS
{
    [TestClass]
    public class OperatorTests : BaseSASTests
    {
        [TestMethod]
        public void Can_GenerateConstructorRefs()
        {
            // ARRANGE
            var pre = GenerateRandomFacts(0, 5);
            var add = GenerateRandomFacts(3, 5);
            var del = GenerateRandomFacts(10, 5);

            // ACT
            var op = new Operator("op", new string[0], pre.ToArray(), add.ToArray(), del.ToArray());

            // ASSERT
            Assert.AreEqual(pre.Count, op.Pre.Length);
            Assert.AreEqual(pre.Count, op.PreRef.Count);
            foreach (var item in pre)
            {
                Assert.IsTrue(op.PreRef.Contains(item.ID));
                Assert.IsTrue(op.Pre.Contains(item));
            }

            Assert.AreEqual(add.Count, op.Add.Length);
            Assert.AreEqual(add.Count, op.AddRef.Count);
            foreach (var item in add)
            {
                Assert.IsTrue(op.AddRef.Contains(item.ID));
                Assert.IsTrue(op.Add.Contains(item));
            }

            Assert.AreEqual(del.Count, op.Del.Length);
            Assert.AreEqual(del.Count, op.DelRef.Count);
            foreach (var item in del)
            {
                Assert.IsTrue(op.DelRef.Contains(item.ID));
                Assert.IsTrue(op.Del.Contains(item));
            }
        }

        [TestMethod]
        [DataRow(0, 1)]
        [DataRow(0, 10)]
        [DataRow(0, 10000)]
        public void Can_Equals_IfTrue(int from, int count)
        {
            // ARRANGE
            var ops1 = GenerateRandomOperator(from, count);
            var ops2 = GenerateRandomOperator(from, count);

            // ACT
            // ASSERT
            for (int i = 0; i < ops1.Count; i++)
                Assert.IsTrue(ops1[i].Equals(ops2[i]));
        }

        [TestMethod]
        [DataRow(0, 100, 100)]
        [DataRow(10, 30, 1)]
        [DataRow(10, 20, 5)]
        public void Can_Equals_IfFalse(int from1, int from2, int count)
        {
            // ARRANGE
            var ops1 = GenerateRandomOperator(from1, count);
            var ops2 = GenerateRandomOperator(from2, count);

            // ACT
            // ASSERT
            for (int i = 0; i < ops1.Count; i++)
                Assert.IsFalse(ops1[i].Equals(ops2[i]));
        }

        [TestMethod]
        public void Can_Copy()
        {
            // ARRANGE
            var ops1 = GenerateRandomOperator(0, 100);
            var ops2 = new List<Operator>();

            // ACT
            foreach (var op in ops1)
                ops2.Add(op.Copy());

            // ASSERT
            for (int i = 0; i < ops1.Count; i++)
                Assert.IsTrue(ops1[i].Equals(ops2[i]));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(100)]
        [DataRow(1000)]
        public void Can_GetHashCode(int count)
        {
            // ARRANGE
            var ops1 = GenerateRandomOperator(0, count);

            // ACT
            // ASSERT
            Assert.AreEqual(ops1.Count, ops1.DistinctBy(x => x.GetHashCode()).Count());
        }
    }
}
