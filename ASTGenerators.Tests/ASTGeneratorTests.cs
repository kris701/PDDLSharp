using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.AST;

namespace PDDLSharp.ASTGenerators.Tests
{
    [TestClass]
    public class ASTGeneratorTests
    {
        #region Overall
        [TestMethod]
        [DataRow("()", 1)]
        [DataRow("(abc)", 1)]
        [DataRow("(())", 2)]
        [DataRow("(abc (def))", 2)]
        [DataRow("(()())", 3)]
        [DataRow("(abc (def) (ghi))", 3)]
        [DataRow("(abc (def) (ghi (jkl)))", 4)]
        public void Can_ParseGeneralStructure(string toParse, int expectedNodes)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IGenerator parser = new ASTGenerator(listener);

            // ACT
            var res = parser.Generate(toParse);

            // ASSERT
            Assert.AreEqual(expectedNodes, res.Count);
        }

        [TestMethod]
        [DataRow("(pred ?a ?b)", "(pred ?a ?b)")]
        [DataRow("(pred)", "(pred)")]
        [DataRow("(something thats an invalid pddl item)", "(something thats an invalid pddl item)")]
        [DataRow("?a", "?a")]
        public void Can_ParseSingleNodeContent_1(string toParse, string expectedContent)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IGenerator parser = new ASTGenerator(listener);

            // ACT
            var res = parser.Generate(toParse);

            // ASSERT
            Assert.AreEqual(expectedContent, res.OuterContent);
        }

        [TestMethod]
        [DataRow("(pred ?a ?b)", "pred ?a ?b")]
        [DataRow("(pred)", "pred")]
        [DataRow("(something thats an invalid pddl item)", "something thats an invalid pddl item")]
        [DataRow("?a", "?a")]
        public void Can_ParseSingleNodeContent_2(string toParse, string expectedContent)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IGenerator parser = new ASTGenerator(listener);

            // ACT
            var res = parser.Generate(toParse);

            // ASSERT
            Assert.AreEqual(expectedContent, res.InnerContent);
        }

        [TestMethod]
        [DataRow("((pred (not (var ?a)))\n(pred (not (var ?a))))", 0, 1)]
        [DataRow("((pred (not (var ?a)))\n(pred (not (var ?a))))", 1, 2)]
        [DataRow("((pred (not (var ?a)))\n(pred (not (var ?a)))\n(pred (not (var ?a))))", 2, 3)]
        [DataRow("((pred (not (var ?a)))\n(pred (not (var ?a)))\n\n\n(pred (not (var ?a))))", 2, 5)]
        [DataRow("\n\n\n((pred (not (var ?a)))\n(pred (not (var ?a))))\n\n\n(pred (not (var ?a)))", 2, 8)]
        public void Can_CanSetCorrectLineNumber(string toParse, int targetChild, int expectedLineNumber)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IGenerator parser = new ASTGenerator(listener);

            // ACT
            var res = parser.Generate(toParse);

            // ASSERT
            Assert.AreEqual(expectedLineNumber, res.Children[targetChild].Line);
        }
        #endregion

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
            ASTGenerator parser = new ASTGenerator(listener);

            // ACT
            var res = parser.GenerateLineDict(text);

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
            IErrorListener listener = new ErrorListener();
            ASTGenerator parser = new ASTGenerator(listener);
            var dict = parser.GenerateLineDict(text);

            // ACT
            var res = parser.GetLineNumber(dict, startCharacter, 0);

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
            IErrorListener listener = new ErrorListener();
            ASTGenerator parser = new ASTGenerator(listener);
            var dict = parser.GenerateLineDict(text);

            // ACT
            var res = parser.GetLineNumber(dict, startCharacter, offset);

            // ASSERT
            Assert.AreEqual(expected, res);
        }

        #endregion
    }
}