using PDDLSharp;
using PDDLSharp.ASTGenerators;
using PDDLSharp.ASTGenerators.PDDL;
using PDDLSharp.ASTGenerators.Tests;
using PDDLSharp.ASTGenerators.Tests.PDDL;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.AST;

namespace PDDLSharp.ASTGenerators.Tests.PDDL
{
    [TestClass]
    public class PDDLASTGeneratorTests
    {
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
            IGenerator parser = new PDDLASTGenerator(listener);

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
            IGenerator parser = new PDDLASTGenerator(listener);

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
            IGenerator parser = new PDDLASTGenerator(listener);

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
            IGenerator parser = new PDDLASTGenerator(listener);

            // ACT
            var res = parser.Generate(toParse);

            // ASSERT
            Assert.AreEqual(expectedLineNumber, res.Children[targetChild].Line);
        }
    }
}