using PDDLSharp;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit;
using PDDLSharp.Toolkit.Planners;
using PDDLSharp.Toolkit.Planners.Heuristics;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.Planners.Tests;
using PDDLSharp.Toolkit.Planners.Tests.Heuristics;
using PDDLSharp.Toolkit.Planners.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Tests.Heuristics
{
    [TestClass]
    public class hWeightedTests
    {
        [TestMethod]
        [DataRow(1, 1, 1)]
        [DataRow(1, 2, 2)]
        [DataRow(50, 2, 100)]
        [DataRow(50, 0.5, 25)]
        public void Can_GeneratehWeightedCorrectly(int constant, double weight, int expected)
        {
            // ARRANGE
            IHeuristic h = new hWeighted(new hConstant(constant), weight);
            var parent = new StateMove();

            // ACT
            var newValue = h.GetValue(parent, null, new List<Operator>());

            // ASSERT
            Assert.AreEqual(expected, newValue);
        }
    }
}
