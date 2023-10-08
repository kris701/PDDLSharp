using PDDLSharp;
using PDDLSharp.ASTGenerators;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.Tests;
using PDDLSharp.Parsers.Tests;
using PDDLSharp.Parsers.Tests.PositionTestsData;
using PDDLSharp.Parsers.Tests.Visitors;
using PDDLSharp.Parsers.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Parsers.Tests
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
            var expectedNode = PositionNode.ParseExpectedFile(expectedFile);
            IErrorListener listener = new ErrorListener();
            IParser<INode> pddlParser = new PDDLParser(listener);

            // ACT
            var node = pddlParser.ParseAs<DomainDecl>(testFile);

            // ASSERT
            IsNodePositionValid(node, expectedNode);
        }

        private void IsNodePositionValid(INode node, PositionNode expectedNode)
        {
            Assert.AreEqual(expectedNode.Start, node.Start, $"Start did not match in node '{expectedNode.NodeType}'");
            Assert.AreEqual(expectedNode.End, node.End, $"End did not match in node '{expectedNode.NodeType}'");
            Assert.AreEqual(expectedNode.NodeType, node.GetType().Name, $"Type did not match in node '{expectedNode.NodeType}'");

            if (node is IWalkable walkable)
            {
                int index = 0;
                foreach (var child in walkable)
                {
                    IsNodePositionValid(child, expectedNode.Children[index++]);
                }
            }
        }
    }
}
