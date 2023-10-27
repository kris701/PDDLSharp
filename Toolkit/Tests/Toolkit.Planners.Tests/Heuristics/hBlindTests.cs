using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Toolkit.Planners.Heuristics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Tests.Heuristics
{
    [TestClass]
    public class hBlindTests
    {
        [TestMethod]
        [DataRow(1, 1 - 1)]
        [DataRow(int.MaxValue, int.MaxValue - 1)]
        [DataRow(62362524, 62362524 - 1)]
        [DataRow(-62362524, -62362524 - 1)]
        public void Can_GeneratehBlindCorrectly(int inValue, int expected)
        {
            // ARRANGE
            IHeuristic h = new hBlind(new PDDLDecl());

            // ACT
            var newValue = h.GetValue(inValue, null, new HashSet<ActionDecl>());

            // ASSERT
            Assert.AreEqual(expected, newValue);
        }
    }
}
