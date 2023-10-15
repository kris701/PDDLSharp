using PDDLSharp.ASTGenerators.PDDL;
using PDDLSharp.ErrorListeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.ASTGenerators.Tests
{
    [TestClass]
    public class BaseASTGeneratorTests
    {
        #region GenerateLineDict

        [TestMethod]
        [DataRow("abc\ndef\n", 3, 7)]
        [DataRow("abc\ndeaaaaaaaaaf\n", 3, 16)]
        [DataRow("abc\ndeaaaaaaaaaf\naaa\n", 3, 16)]
        [DataRow("abc\ndeaaaaaaaaaf\naaa\n", 3, 16, 20)]
        public void Can_GenerateLineDict(string text, params int[] expected)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();

            // ACT
            var res = BaseASTGenerator.GenerateLineDict(text, '\n');

            // ASSERT
            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], res[i]);
        }

        #endregion

        #region GetLineNumber

        [TestMethod]
        [DataRow("abc\ndef\n", 2, 1)]
        [DataRow("abc\ndef\n", 3, 2)]
        [DataRow("abc\ndef\n", 4, 2)]
        [DataRow("abc\ndef\n", 6, 2)]
        [DataRow("abc\ndeaaaaaaaaaf\n", 2, 1)]
        [DataRow("abc\ndeaaaaaaaaaf\n", 10, 2)]
        [DataRow("abc\ndeaaaaaaaaaf\naaa\n", 18, 3)]
        public void Can_GetLineNumber(string text, int startCharacter, int expected)
        {
            // ARRANGE
            var dict = BaseASTGenerator.GenerateLineDict(text, '\n');

            // ACT
            var res = BaseASTGenerator.GetLineNumber(dict, startCharacter, 0);

            // ASSERT
            Assert.AreEqual(expected, res);
        }

        [TestMethod]
        [DataRow("abc\ndef\n", 2, 0, 1)]
        [DataRow("abc\ndef\n", 5, 1, 2)]
        [DataRow("abc\ndeaaaaaaaaaf\naaa\n", 18, 2, 3)]
        public void Can_GetLineNumber_WithOffset(string text, int startCharacter, int offset, int expected)
        {
            // ARRANGE
            var dict = BaseASTGenerator.GenerateLineDict(text, '\n');

            // ACT
            var res = BaseASTGenerator.GetLineNumber(dict, startCharacter, offset);

            // ASSERT
            Assert.AreEqual(expected, res);
        }

        #endregion
    }
}
