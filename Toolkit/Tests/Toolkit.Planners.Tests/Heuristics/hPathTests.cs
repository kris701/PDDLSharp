using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Planners.Heuristics;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.Planners.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Tests.Heuristics
{
    [TestClass]
    public class hPathTests
    {
        [TestMethod]
        [DataRow(1, 1 + 1)]
        [DataRow(20, 20 + 1)]
        [DataRow(566, 566 + 1)]
        public void Can_GeneratehPathCorrectly(int inValue, int expected)
        {
            // ARRANGE
            IHeuristic h = new hPath();
            var parent = new StateMove();
            for (int i = 0; i < inValue; i++)
                parent.Steps.Add(new Operator());

            // ACT
            var newValue = h.GetValue(parent, null, new List<Operator>());

            // ASSERT
            Assert.AreEqual(expected, newValue);
        }
    }
}
