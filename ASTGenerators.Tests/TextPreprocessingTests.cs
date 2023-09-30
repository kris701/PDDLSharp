using PDDLSharp.ASTGenerators.Tests.PositionTestsData;
using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.ASTGenerators.Tests
{
    [TestClass]
    public class TextPreprocessingTests
    {
        [TestMethod]
        [DataRow("", "")]
        [DataRow("\r", " ")]
        [DataRow("\r \t \r", "     ")]
        [DataRow("\reyayaya\taaaa\r", " eyayaya aaaa ")]
        public void Can_ReplaceSpecialCharacters(string text, string expectedText)
        {
            // ARRANGE
            // ACT
            var result = TextPreprocessing.ReplaceSpecialCharacters(text);

            // ASSERT
            Assert.AreEqual(expectedText, result);
        }

        [TestMethod]
        [DataRow("abc\n", "abc\n")]
        [DataRow(";abc\n", "    \n")]
        [DataRow(";abc\nabc\n", "    \nabc\n")]
        [DataRow(";abc\nabc\n;aaa\n", "    \nabc\n    \n")]
        public void Can_ReplaceCommentsWithWhiteSpace(string text, string expectedText)
        {
            // ARRANGE
            // ACT
            var result = TextPreprocessing.ReplaceCommentsWithWhiteSpace(text);

            // ASSERT
            Assert.AreEqual(expectedText, result);
        }

        [TestMethod]
        [DataRow("abc", "abc")]
        [DataRow("abc - type", "abc=t=type")]
        [DataRow("abc - type\nttt - otherType", "abc=t=type\nttt=t=otherType")]
        [DataRow("abc\n- type", "abc\n=t=type")]
        public void Can_TokenizeSpecials(string text, string expectedText)
        {
            // ARRANGE
            // ACT
            var result = TextPreprocessing.TokenizeSpecials(text);

            // ASSERT
            Assert.AreEqual(expectedText, result);
        }
    }
}
