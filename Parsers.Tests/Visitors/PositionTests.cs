﻿using PDDL.ASTGenerator;
using PDDL.ErrorListeners;
using PDDL.Models;
using PDDL.Models.AST;
using PDDL.Models.Domain;
using PDDL.Parsers.Tests.PositionTestsData;
using PDDL.Parsers.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDL.Parsers.Tests.Visitors
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
            IPDDLParser pddlParser = new PDDLParser();

            // ACT
            var node = pddlParser.ParseDomain(testFile);

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
                    //if (index >= expectedNode.Children.Count)
                    //    Assert.Fail($"Node did not have the expected number of children! Node {expectedNode.NodeType}, expected {expectedNode.Children.Count} children");
                    
                    IsNodePositionValid(child, expectedNode.Children[index++]);
                }
            }
        }
    }
}
