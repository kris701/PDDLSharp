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
    public class AnalyserVisitorsTests : BaseVisitorsTests
    {
        [TestMethod]
        [DataRow("TestFiles/gripper-domain-badaction.pddl", "TestFiles/gripper-prob01.pddl", 2, 0, 3)]
        [DataRow("TestFiles/gripper-domain-badaction.pddl", "TestFiles/gripper-prob01.pddl", 0, 0, 4)]
        [DataRow("TestFiles/gripper-domain-badaction.pddl", "TestFiles/gripper-prob01.pddl", 0, 0, 5)]
        [DataRow("TestFiles/gripper-domain-badaction.pddl", "TestFiles/gripper-prob01.pddl", 2, 0)]
        [DataRow("TestFiles/gripper-domain-badaction.pddl", "TestFiles/gripper-prob01.pddl", 0, 1)]
        [DataRow("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl", 0, 0, 3)]
        [DataRow("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl", 0, 0, 4)]
        [DataRow("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl", 0, 0, 5)]
        [DataRow("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl", 0, 0)]
        [DataRow("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl", 0, 1)]
        [DataRow("TestFiles/satellite-domain.pddl", "TestFiles/satellite-prob01.pddl", 0, 0)]
        [DataRow("TestFiles/satellite-domain.pddl", "TestFiles/satellite-prob01.pddl", 0, 1)]
        [DataRow("TestFiles/agricola-domain.pddl", "TestFiles/agricola-prob01.pddl", 0, 0)]
        [DataRow("TestFiles/agricola-domain.pddl", "TestFiles/agricola-prob01.pddl", 0, 1)]
        [DataRow("TestFiles/agricola-domain-badaction.pddl", "TestFiles/agricola-prob01.pddl", 0, 0, 9)]
        [DataRow("TestFiles/agricola-domain-badaction.pddl", "TestFiles/agricola-prob01.pddl", 2, 0, 8)]
        [DataRow("TestFiles/agricola-domain-badaction.pddl", "TestFiles/agricola-prob01.pddl", 0, 0, 7)]
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

        [TestMethod]
        [DataRow("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl", 0, 0)]
        [DataRow("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl", 0, 1)]
        [DataRow("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01-undeclaredgoalpredicate.pddl", 1, 1)]
        [DataRow("TestFiles/satellite-domain.pddl", "TestFiles/satellite-prob01.pddl", 0, 0)]
        [DataRow("TestFiles/satellite-domain.pddl", "TestFiles/satellite-prob01.pddl", 0, 1)]
        [DataRow("TestFiles/agricola-domain.pddl", "TestFiles/agricola-prob01.pddl", 0, 0)]
        [DataRow("TestFiles/agricola-domain.pddl", "TestFiles/agricola-prob01.pddl", 0, 1)]
        [DataRow("TestFiles/agricola-domain-undeclaredactionpredicate.pddl", "TestFiles/agricola-prob01.pddl", 0, 0, 10)]
        [DataRow("TestFiles/agricola-domain-undeclaredactionpredicate.pddl", "TestFiles/agricola-prob01.pddl", 1, 0, 11)]
        [DataRow("TestFiles/agricola-domain-undeclaredactionpredicate.pddl", "TestFiles/agricola-prob01.pddl", 0, 0, 12)]
        [DataRow("TestFiles/agricola-domain.pddl", "TestFiles/agricola-prob01-undeclaredinitpredicate.pddl", 0, 1, 2)]
        [DataRow("TestFiles/agricola-domain.pddl", "TestFiles/agricola-prob01-undeclaredinitpredicate.pddl", 1, 1, 3)]
        [DataRow("TestFiles/agricola-domain.pddl", "TestFiles/agricola-prob01-undeclaredinitpredicate.pddl", 0, 1, 4)]
        public void Can_CheckForUndeclaredPredicates(string domain, string problem, int expectedErrors, params int[] targetNode)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);
            var decl = GetDeclaration(domain, problem, listener);
            Assert.IsNotNull(decl);
            var node = GetNode(decl, targetNode, listener) as IWalkable;
            Assert.IsNotNull(node);
            var analyser = new AnalyserVisitors(listener, decl);

            // ACT
            analyser.CheckForUndeclaredPredicates(
                node,
                (pred) => new PDDLSharpError(
                    $"Err",
                    ParseErrorType.Error,
                    ParseErrorLevel.Analyser,
                    node.Line,
                    node.Start));

            // ASSERT
            Assert.AreEqual(expectedErrors, listener.CountErrorsOfTypeOrAbove(ParseErrorType.Warning));
        }

        [TestMethod]
        [DataRow("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl", 0, 0, 1)]
        [DataRow("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl", 0, 0, 2)]
        [DataRow("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl", 0, 1, 2)]
        [DataRow("TestFiles/satellite-domain.pddl", "TestFiles/satellite-prob01.pddl", 0, 0, 2)]
        [DataRow("TestFiles/satellite-domain.pddl", "TestFiles/satellite-prob01.pddl", 0, 1, 2)]
        [DataRow("TestFiles/satellite-domain.pddl", "TestFiles/satellite-prob01-duplicateobjects.pddl", 2, 1, 2)]
        [DataRow("TestFiles/agricola-domain.pddl", "TestFiles/agricola-prob01.pddl", 0, 0, 2)]
        [DataRow("TestFiles/agricola-domain.pddl", "TestFiles/agricola-prob01.pddl", 0, 0, 3)]
        [DataRow("TestFiles/agricola-domain.pddl", "TestFiles/agricola-prob01.pddl", 0, 0, 4)]
        [DataRow("TestFiles/agricola-domain.pddl", "TestFiles/agricola-prob01.pddl", 0, 0, 5)]
        [DataRow("TestFiles/agricola-domain.pddl", "TestFiles/agricola-prob01.pddl", 0, 1, 2)]
        public void Can_CheckForUniqueNames(string domain, string problem, int expectedErrors, params int[] targetNode)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);
            var decl = GetDeclaration(domain, problem, listener);
            Assert.IsNotNull(decl);
            var node = GetNode(decl, targetNode, listener) as IWalkable;
            Assert.IsNotNull(node);
            var analyser = new AnalyserVisitors(listener, decl);

            // ACT
            analyser.CheckForUniqueNames(
                node,
                (pred) => new PDDLSharpError(
                    $"Err",
                    ParseErrorType.Error,
                    ParseErrorLevel.Analyser,
                    node.Line,
                    node.Start));

            // ASSERT
            Assert.AreEqual(expectedErrors, listener.CountErrorsOfTypeOrAbove(ParseErrorType.Warning));
        }

        [TestMethod]
        [DataRow("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl", 0, 0, 3)]
        [DataRow("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl", 0, 0, 4)]
        [DataRow("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl", 0, 0, 5)]
        [DataRow("TestFiles/gripper-domain-unusedparamsaction.pddl", "TestFiles/gripper-prob01.pddl", 0, 0, 3)]
        [DataRow("TestFiles/gripper-domain-unusedparamsaction.pddl", "TestFiles/gripper-prob01.pddl", 1, 0, 4)]
        [DataRow("TestFiles/gripper-domain-unusedparamsaction.pddl", "TestFiles/gripper-prob01.pddl", 2, 0, 5)]
        public void Can_CheckForUnusedParameters(string domain, string problem, int expectedErrors, params int[] targetNode)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);
            var decl = GetDeclaration(domain, problem, listener);
            Assert.IsNotNull(decl);
            var node = GetNode(decl, targetNode, listener) as IParametized;
            Assert.IsNotNull(node);
            var analyser = new AnalyserVisitors(listener, decl);

            // ACT
            analyser.CheckForUnusedParameters(
                node,
                (pred) => new PDDLSharpError(
                    $"Err",
                    ParseErrorType.Error,
                    ParseErrorLevel.Analyser,
                    node.Line,
                    node.Start));

            // ASSERT
            Assert.AreEqual(expectedErrors, listener.CountErrorsOfTypeOrAbove(ParseErrorType.Warning));
        }

        [TestMethod]
        [DataRow("TestFiles/satellite-domain.pddl", "TestFiles/satellite-prob01.pddl", 0, 0, 3)]
        [DataRow("TestFiles/satellite-domain.pddl", "TestFiles/satellite-prob01.pddl", 0, 0, 4)]
        [DataRow("TestFiles/satellite-domain.pddl", "TestFiles/satellite-prob01.pddl", 0, 0, 5)]
        [DataRow("TestFiles/satellite-domain.pddl", "TestFiles/satellite-prob01.pddl", 0, 0, 6)]
        [DataRow("TestFiles/satellite-domain.pddl", "TestFiles/satellite-prob01.pddl", 0, 0, 7)]
        [DataRow("TestFiles/satellite-domain-undeclaredparameter.pddl", "TestFiles/satellite-prob01.pddl", 0, 0, 3)]
        [DataRow("TestFiles/satellite-domain-undeclaredparameter.pddl", "TestFiles/satellite-prob01.pddl", 1, 0, 4)]
        [DataRow("TestFiles/satellite-domain-undeclaredparameter.pddl", "TestFiles/satellite-prob01.pddl", 0, 0, 5)]
        [DataRow("TestFiles/satellite-domain-undeclaredparameter.pddl", "TestFiles/satellite-prob01.pddl", 0, 0, 6)]
        [DataRow("TestFiles/satellite-domain-undeclaredparameter.pddl", "TestFiles/satellite-prob01.pddl", 0, 0, 7)]
        [DataRow("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl", 0, 0, 3)]
        [DataRow("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl", 0, 0, 4)]
        [DataRow("TestFiles/gripper-domain.pddl", "TestFiles/gripper-prob01.pddl", 0, 0, 5)]
        [DataRow("TestFiles/gripper-domain-undeclaredparameter.pddl", "TestFiles/gripper-prob01.pddl", 0, 0, 3)]
        [DataRow("TestFiles/gripper-domain-undeclaredparameter.pddl", "TestFiles/gripper-prob01.pddl", 1, 0, 4)]
        [DataRow("TestFiles/gripper-domain-undeclaredparameter.pddl", "TestFiles/gripper-prob01.pddl", 0, 0, 5)]
        public void Can_CheckForUndeclaredParameters(string domain, string problem, int expectedErrors, params int[] targetNode)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);
            var decl = GetDeclaration(domain, problem, listener);
            Assert.IsNotNull(decl);
            var node = GetNode(decl, targetNode, listener) as IParametized;
            Assert.IsNotNull(node);
            var analyser = new AnalyserVisitors(listener, decl);

            // ACT
            analyser.CheckForUndeclaredParameters(
                node,
                (pred) => new PDDLSharpError(
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
