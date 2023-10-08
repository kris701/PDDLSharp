using PDDLSharp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.ToolsTests
{
    [TestClass]
    public class StringHelperTests
    {

        #region ReplaceRangeWithSpaces

        [TestMethod]
        //        123456789
        [DataRow("(test)", 1, 5, "(    )")]
        [DataRow("(test)", 0, 6, "      ")]
        [DataRow("(test (aba))", 6, 11, "(test      )")]
        public void Can_ReplaceRangeWithSpaces(string text, int from, int to, string expected)
        {
            // ARRANGE

            // ACT
            var res = StringHelpers.ReplaceRangeWithSpacesFast(text, from, to);

            // ASSERT
            Assert.AreEqual(expected, res);
        }

        #endregion
    }
}
