using PDDL.ASTGenerator.Tests.PositionTestsData;
using PDDL.Models.AST;

namespace PDDL.ASTGenerator.Tests
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
            IASTParser<ASTNode> parser = new ASTParser();
            var expectedNode = PositionNode.ParseExpectedFile(expectedFile);

            // ACT
            var node = parser.Parse(testFile);

            // ASSERT
            IsNodePositionValid(node, expectedNode);
        }

        private void IsNodePositionValid(ASTNode node, PositionNode expectedNode)
        {
            Assert.AreEqual(expectedNode.Start, node.Start, $"Expected: {expectedNode.NodeType}, Got: {node.InnerContent}");
            Assert.AreEqual(expectedNode.End, node.End, $"Expected: {expectedNode.NodeType}, Got: {node.InnerContent}");
            if (expectedNode.Children.Count == node.Children.Count)
                for (int i = 0; i < expectedNode.Children.Count; i++)
                    IsNodePositionValid(node.Children[i], expectedNode.Children[i]);
        }
    }
}
