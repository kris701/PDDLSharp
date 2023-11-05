using PDDLSharp.Models.SAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.Tests.SAS
{
    [TestClass]
    public class OperatorTests
    {
        private List<Operator> GenerateRandomOperator(int fromID, int amount)
        {
            var ops = new List<Operator>();
            var rnd = new Random();
            for (int i = 0; i < amount; i++)
            {
                var newOp = new Operator($"fact-{fromID + i}", new string[0], new Fact[0], new Fact[0], new Fact[0]);
                newOp.ID = fromID + i;
                ops.Add(newOp);
            }

            return ops;
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
        [DataRow(10000)]
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
