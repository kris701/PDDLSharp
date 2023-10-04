using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PDDLSharp.Simulators.StateSpace.Tests
{
    [TestClass]
    public class OperatorTests
    {
        [DataRow("pred", "prad")]
        [DataRow("pred", "prud", "obja")]
        [DataRow("pred", "q", "obja", "aaa")]
        [DataRow("a", "b", "obja", "aaa")]
        [DataRow("a", "b", "o214213bja", "aaa")]
        [DataRow("a", "b", "o214213bja", "aaa", "qqq")]
        public void Can_CheckIfNotEqual(string name1, string name2, params string[] args)
        {
            // ARRANGE
            var op1 = new Operator(name1, args);
            var op2 = new Operator(name2, args);

            // ACT
            // ASSERT
            Assert.IsFalse(op1.Equals(op2));
            Assert.AreNotEqual(op1.GetHashCode(), op2.GetHashCode());
        }

        [TestMethod]
        [DataRow("pred")]
        [DataRow("pred", "obja")]
        [DataRow("pred", "obja", "aaa")]
        [DataRow("a", "obja", "aaa")]
        [DataRow("a", "o214213bja", "aaa")]
        [DataRow("a", "o214213bja", "aaa", "qqq")]
        public void Can_CheckIfEqual(string name, params string[] args)
        {
            // ARRANGE
            var op1 = new Operator(name, args);
            var op2 = new Operator(name, args);

            // ACT
            // ASSERT
            Assert.IsTrue(op1.Equals(op2));
            Assert.AreEqual(op1.GetHashCode(), op2.GetHashCode());
        }
    }
}
