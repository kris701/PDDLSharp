using PDDLSharp.Models.SAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.Tests.SAS
{
    [TestClass]
    public class FactTests
    {
        private List<Fact> GenerateRandomFacts(int fromID, int amount)
        {
            var facts = new List<Fact>();
            var rnd = new Random();
            for (int i = 0; i < amount; i++)
            {
                var newFact = new Fact($"fact-{fromID + i}");
                newFact.ID = fromID + i;
                facts.Add(newFact);
            }

            return facts;
        }

        [TestMethod]
        public void Can_Equals_IfTrue()
        {
            // ARRANGE
            var facts1 = GenerateRandomFacts(0, 100);
            var facts2 = GenerateRandomFacts(0, 100);

            // ACT
            // ASSERT
            for (int i = 0; i < facts1.Count; i++)
                Assert.IsTrue(facts1[i].Equals(facts2[i]));
        }

        [TestMethod]
        public void Can_Equals_IfFalse()
        {
            // ARRANGE
            var facts1 = GenerateRandomFacts(0, 100);
            var facts2 = GenerateRandomFacts(500, 100);

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
    }
}
