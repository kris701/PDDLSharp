using PDDLSharp.Models.SAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.Tests.SAS
{
    [TestClass]
    public class FactTests : BaseSASTests
    {
        [TestMethod]
        [DataRow(0, 1)]
        [DataRow(0, 10)]
        [DataRow(0, 10000)]
        public void Can_Equals_IfTrue(int from, int count)
        {
            // ARRANGE
            var facts1 = GenerateRandomFacts(from, count);
            var facts2 = GenerateRandomFacts(from, count);

            // ACT
            // ASSERT
            for (int i = 0; i < facts1.Count; i++)
                Assert.IsTrue(facts1[i].Equals(facts2[i]));
        }

        [TestMethod]
        [DataRow(0, 100, 100)]
        [DataRow(10, 30, 1)]
        [DataRow(10, 20, 5)]
        public void Can_Equals_IfFalse(int from1, int from2, int count)
        {
            // ARRANGE
            var facts1 = GenerateRandomFacts(from1, count);
            var facts2 = GenerateRandomFacts(from2, count);

            // ACT
            // ASSERT
            for (int i = 0; i < facts1.Count; i++)
                Assert.IsFalse(facts1[i].Equals(facts2[i]));
        }

        [TestMethod]
        public void Can_Copy()
        {
            // ARRANGE
            var facts1 = GenerateRandomFacts(0, 100);
            var facts2 = new List<Fact>();

            // ACT
            foreach (var fact in facts1)
                facts2.Add(fact.Copy());

            // ASSERT
            for (int i = 0; i < facts1.Count; i++)
                Assert.IsTrue(facts1[i].Equals(facts2[i]));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(100)]
        [DataRow(1000)]
        public void Can_GetHashCode(int count)
        {
            // ARRANGE
            var facts = GenerateRandomFacts(0, count);

            // ACT
            // ASSERT
            Assert.AreEqual(facts.Count, facts.DistinctBy(x => x.GetHashCode()).Count());
        }
    }
}
