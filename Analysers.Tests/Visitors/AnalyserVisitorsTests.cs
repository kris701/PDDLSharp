using PDDLSharp.Analysers.Visitors;
using PDDLSharp.Contextualisers;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.Plans;
using PDDLSharp.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Analysers.Tests.Visitors
{
    [TestClass]
    public class AnalyserVisitorsTests
    {
        private PDDLDecl GetDeclaration(string domain, string problem, IErrorListener listener)
        {
            PDDLParser parser = new PDDLParser(listener);
            var decl = parser.ParseDecl(domain, problem);
            IContextualiser contextualiser = new PDDLContextualiser(listener);
            contextualiser.Contexturalise(decl);
            return decl;
        }

        private INode GetNode(PDDLDecl decl, int[] target, IErrorListener listener)
        {
            if (target[0] == 0)
                return GetNode(decl.Domain, 1, target, listener);
            if (target[0] == 1)
                return GetNode(decl.Problem, 1, target, listener);
            return null;
        }

        private INode GetNode(IWalkable source, int index, int[] target, IErrorListener listener)
        {
            int counter = 0;
            foreach(var item in source)
            {
                if (index == target.Length && target[index] == counter)
                    return item;
                else if (target[index] == counter)
                    return item;
                counter++;
            }
            return null;
        }

        [TestMethod]
        [DataRow("TestFiles/gripper-domain-badaction.pddl", "TestFiles/gripper-prob01.pddl", 2, 0, 3)]
        [DataRow("TestFiles/gripper-domain-badaction.pddl", "TestFiles/gripper-prob01.pddl", 0, 0, 4)]
        [DataRow("TestFiles/gripper-domain-badaction.pddl", "TestFiles/gripper-prob01.pddl", 0, 0, 5)]
        [DataRow("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl", 0, 0, 3)]
        [DataRow("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl", 0, 0, 4)]
        [DataRow("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl", 0, 0, 5)]
        public void Can_CheckForCorrectPredicateTypes(string domain, string problem, int expectedErrors, params int[] targetNode)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);
            var decl = GetDeclaration(domain, problem, listener);
            Assert.IsNotNull(decl);
            var node = GetNode(decl, targetNode, listener) as IWalkable;
            Assert.IsNotNull(node);
            var analyser = new AnalyserVisitors(listener, decl);

            // ACT
            analyser.CheckForCorrectPredicateTypes(
                node,
                (pred, expected, actual) => new PDDLSharpError(
                    $"Err",
                    ParseErrorType.Error,
                    ParseErrorLevel.Analyser,
                    node.Line,
                    node.Start));

            // ASSERT
            Assert.AreEqual(expectedErrors, listener.CountErrorsOfTypeOrAbove(ParseErrorType.Warning));
        }
    }
}
