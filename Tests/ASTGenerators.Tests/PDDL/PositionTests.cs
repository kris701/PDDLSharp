using PDDLSharp;
using PDDLSharp.ASTGenerators;
using PDDLSharp.ASTGenerators.PDDL;
using PDDLSharp.ASTGenerators.Tests;
using PDDLSharp.ASTGenerators.Tests.PDDL;
using PDDLSharp.ASTGenerators.Tests.PDDL.PositionTestsData;
using PDDLSharp.ASTGenerators.Tests.PositionTestsData;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.AST;

namespace PDDLSharp.ASTGenerators.Tests.PDDL
{
    [TestClass]
    public class PositionTests
    {
        [TestMethod]
        [DataRow("PositionTestsData/gripper-domain.pddl", "PositionTestsData/gripper-domain-expected.txt")]
        [DataRow("PositionTestsData/construction-domain.pddl", "PositionTestsData/construction-domain-expected.txt")]
        public void Can_ASTParser_SetCorrectPossitions(string testFile, string expectedFile)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IGenerator parser = new PDDLASTGenerator(listener);
            var expectedNode = PositionNode.ParseExpectedFile(expectedFile);

            // ACT
            var node = parser.Generate(new FileInfo(testFile));

            // ASSERT
            IsNodePositionValid(node, expectedNode);
        }

        private void IsNodePositionValid(ASTNode node, PositionNode expectedNode)
        {
            Assert.AreEqual(expectedNode.Start, node.Start, $"Expected: {expectedNode.NodeType} start, Got: {node.InnerContent}");
            Assert.AreEqual(expectedNode.End, node.End, $"Expected: {expectedNode.NodeType} end, Got: {node.InnerContent}");
            if (expectedNode.Children.Count == node.Children.Count)
                for (int i = 0; i < expectedNode.Children.Count; i++)
                    IsNodePositionValid(node.Children[i], expectedNode.Children[i]);
        }
    }
}
