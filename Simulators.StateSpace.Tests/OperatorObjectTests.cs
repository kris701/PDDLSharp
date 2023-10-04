using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Simulators.StateSpace.Tests
{
    [TestClass]
    public class OperatorObjectTests
    {
        [TestMethod]
        [DataRow("obja", "type", "objb", "type")]
        [DataRow("objb", "typ", "objb", "type")]
        [DataRow("objb", "typ", "aw333", "type")]
        [DataRow("obj", "typ", "obj", "type")]
        public void Can_CheckIfNotEqual(string name1, string type1, string name2, string type2)
        {
            // ARRANGE
            var op1 = new OperatorObject(name1, type1);
            var op2 = new OperatorObject(name2, type2);

            // ACT
            // ASSERT
            Assert.IsFalse(op1.Equals(op2));
            Assert.AreNotEqual(op1.GetHashCode(), op2.GetHashCode());
        }

        [TestMethod]
        [DataRow("obja", "type")]
        [DataRow("objb", "typ")]
        [DataRow("objb", "typ")]
        [DataRow("obj", "typ")]
        public void Can_CheckIfEqual(string name1, string type1)
        {
            // ARRANGE
            var op1 = new OperatorObject(name1, type1);
            var op2 = new OperatorObject(name1, type1);

            // ACT
            // ASSERT
            Assert.IsTrue(op1.Equals(op2));
            Assert.AreEqual(op1.GetHashCode(), op2.GetHashCode());
        }
    }
}
