using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Toolkit.Planners.Heuristics;
using PDDLSharp.Toolkit.Planners.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Tests.Heuristics
{
    [TestClass]
    public class hDepthTests
    {
        [TestMethod]
        [DataRow(1, 1 - 1)]
        [DataRow(int.MaxValue, int.MaxValue - 1)]
        [DataRow(62362524, 62362524 - 1)]
        [DataRow(-62362524, -62362524 - 1)]
        public void Can_GeneratehDepthCorrectly(int inValue, int expected)
        {
            // ARRANGE
            IHeuristic h = new hDepth();
            var parent = new StateMove();
            parent.hValue = inValue;

            // ACT
            var newValue = h.GetValue(parent, null, new List<ActionDecl>());

            // ASSERT
            Assert.AreEqual(expected, newValue);
        }
    }
}
