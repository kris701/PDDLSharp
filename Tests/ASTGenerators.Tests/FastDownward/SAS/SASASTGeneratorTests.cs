using PDDLSharp;
using PDDLSharp.ASTGenerators;
using PDDLSharp.ASTGenerators.FastDownward.SAS;
using PDDLSharp.ASTGenerators.Tests;
using PDDLSharp.ASTGenerators.Tests.FastDownward.SAS;
using PDDLSharp.ASTGenerators.Tests.SAS;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.AST;

namespace PDDLSharp.ASTGenerators.Tests.FastDownward.SAS
{
    [TestClass]
    public class SASASTGeneratorTests
    {
        [TestMethod]
        [DataRow("begin_a\n something\n end_a\n", 1)]
        [DataRow("begin_a\n something\n end_a\nbegin_a\n something\n end_a\n", 2)]
        [DataRow("begin_a\n something\n end_a\nbegin_a\n something\n end_a\n        a         begin_a\n something\n end_a\n", 3)]
        public void Can_ParseGeneralStructure(string toParse, int expectedNodes)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IGenerator parser = new SASASTGenerator(listener);

            // ACT
            var res = parser.Generate(toParse);

            // ASSERT
            Assert.AreEqual(expectedNodes, res.Children.Count);
        }

        [TestMethod]
        [DataRow("begin_a\n something\n end_a\n", "begin_a\n something\n end_a")]
        [DataRow("begin_a\n something abc\n end_a\n", "begin_a\n something abc\n end_a")]
        [DataRow("begin_a\n something abc\n 11 end_a\n", "begin_a\n something abc\n 11 end_a")]
        public void Can_ParseSingleNodeContent_1(string toParse, string expectedContent)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IGenerator parser = new SASASTGenerator(listener);

            // ACT
            var res = parser.Generate(toParse);

            // ASSERT
            Assert.AreEqual(expectedContent, res.Children[0].OuterContent);
        }

        [TestMethod]
        [DataRow("begin_a\n something\n end_a\n", "something")]
        [DataRow("begin_a\n something abc\n end_a\n", "something abc")]
        [DataRow("begin_a\n something abc\n 11\n end_a\n", "something abc\n 11")]
        public void Can_ParseSingleNodeContent_2(string toParse, string expectedContent)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IGenerator parser = new SASASTGenerator(listener);

            // ACT
            var res = parser.Generate(toParse);

            // ASSERT
            Assert.AreEqual(expectedContent.Trim(), res.Children[0].InnerContent.Trim());
        }

        [TestMethod]
        [DataRow("begin_a\n something\n end_a\nbegin_a\n something\n end_a\nbegin_a\n something 2\n end_a\n", 0, 1)]
        [DataRow("begin_a\n something\n end_a\nbegin_a\n something\n end_a\nbegin_a\n something 2\n end_a\n", 1, 4)]
        [DataRow("begin_a\n something\n end_a\nbegin_a\n something\n end_a\nbegin_a\n something 2\n end_a\n", 2, 7)]
        public void Can_CanSetCorrectLineNumber(string toParse, int targetChild, int expectedLineNumber)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IGenerator parser = new SASASTGenerator(listener);

            // ACT
            var res = parser.Generate(toParse);

            // ASSERT
            Assert.AreEqual(expectedLineNumber, res.Children[targetChild].Line);
        }
    }
}