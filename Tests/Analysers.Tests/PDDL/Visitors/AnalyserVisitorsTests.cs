using PDDLSharp;
using PDDLSharp.Analysers;
using PDDLSharp.Analysers.Tests;
using PDDLSharp.Analysers.Tests.PDDL.Visitors;
using PDDLSharp.Analysers.Visitors;
using PDDLSharp.Contextualisers;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Analysers.Tests.PDDL.Visitors
{
    [TestClass]
    public class AnalyserVisitorsTests : BaseVisitorsTests
    {
        [TestMethod]
        [DataRow("PDDL/TestFiles/gripper-domain-badaction.pddl", "PDDL/TestFiles/gripper-prob01.pddl", 2, 0, 3)]
        [DataRow("PDDL/TestFiles/gripper-domain-badaction.pddl", "PDDL/TestFiles/gripper-prob01.pddl", 0, 0, 4)]
        [DataRow("PDDL/TestFiles/gripper-domain-badaction.pddl", "PDDL/TestFiles/gripper-prob01.pddl", 0, 0, 5)]
        [DataRow("PDDL/TestFiles/gripper-domain-badaction.pddl", "PDDL/TestFiles/gripper-prob01.pddl", 2, 0)]
        [DataRow("PDDL/TestFiles/gripper-domain-badaction.pddl", "PDDL/TestFiles/gripper-prob01.pddl", 0, 1)]
        [DataRow("PDDL/TestFiles/gripper-domain.pddl", "PDDL/TestFiles/gripper-prob01.pddl", 0, 0, 3)]
        [DataRow("PDDL/TestFiles/gripper-domain.pddl", "PDDL/TestFiles/gripper-prob01.pddl", 0, 0, 4)]
        [DataRow("PDDL/TestFiles/gripper-domain.pddl", "PDDL/TestFiles/gripper-prob01.pddl", 0, 0, 5)]
        [DataRow("PDDL/TestFiles/gripper-domain.pddl", "PDDL/TestFiles/gripper-prob01.pddl", 0, 0)]
        [DataRow("PDDL/TestFiles/gripper-domain.pddl", "PDDL/TestFiles/gripper-prob01.pddl", 0, 1)]
        [DataRow("PDDL/TestFiles/satellite-domain.pddl", "PDDL/TestFiles/satellite-prob01.pddl", 0, 0)]
        [DataRow("PDDL/TestFiles/satellite-domain.pddl", "PDDL/TestFiles/satellite-prob01.pddl", 0, 1)]
        [DataRow("PDDL/TestFiles/agricola-domain.pddl", "PDDL/TestFiles/agricola-prob01.pddl", 0, 0)]
        [DataRow("PDDL/TestFiles/agricola-domain.pddl", "PDDL/TestFiles/agricola-prob01.pddl", 0, 1)]
        [DataRow("PDDL/TestFiles/agricola-domain-badaction.pddl", "PDDL/TestFiles/agricola-prob01.pddl", 0, 0, 9)]
        [DataRow("PDDL/TestFiles/agricola-domain-badaction.pddl", "PDDL/TestFiles/agricola-prob01.pddl", 2, 0, 8)]
        [DataRow("PDDL/TestFiles/agricola-domain-badaction.pddl", "PDDL/TestFiles/agricola-prob01.pddl", 0, 0, 7)]
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
        [DataRow("PDDL/TestFiles/gripper-domain.pddl", "PDDL/TestFiles/gripper-prob01.pddl", 0, 0)]
        [DataRow("PDDL/TestFiles/gripper-domain.pddl", "PDDL/TestFiles/gripper-prob01.pddl", 0, 1)]
        [DataRow("PDDL/TestFiles/gripper-domain.pddl", "PDDL/TestFiles/gripper-prob01-undeclaredgoalpredicate.pddl", 1, 1)]
        [DataRow("PDDL/TestFiles/satellite-domain.pddl", "PDDL/TestFiles/satellite-prob01.pddl", 0, 0)]
        [DataRow("PDDL/TestFiles/satellite-domain.pddl", "PDDL/TestFiles/satellite-prob01.pddl", 0, 1)]
        [DataRow("PDDL/TestFiles/agricola-domain.pddl", "PDDL/TestFiles/agricola-prob01.pddl", 0, 0)]
        [DataRow("PDDL/TestFiles/agricola-domain.pddl", "PDDL/TestFiles/agricola-prob01.pddl", 0, 1)]
        [DataRow("PDDL/TestFiles/agricola-domain-undeclaredactionpredicate.pddl", "PDDL/TestFiles/agricola-prob01.pddl", 0, 0, 10)]
        [DataRow("PDDL/TestFiles/agricola-domain-undeclaredactionpredicate.pddl", "PDDL/TestFiles/agricola-prob01.pddl", 1, 0, 11)]
        [DataRow("PDDL/TestFiles/agricola-domain-undeclaredactionpredicate.pddl", "PDDL/TestFiles/agricola-prob01.pddl", 0, 0, 12)]
        [DataRow("PDDL/TestFiles/agricola-domain.pddl", "PDDL/TestFiles/agricola-prob01-undeclaredinitpredicate.pddl", 0, 1, 2)]
        [DataRow("PDDL/TestFiles/agricola-domain.pddl", "PDDL/TestFiles/agricola-prob01-undeclaredinitpredicate.pddl", 1, 1, 3)]
        [DataRow("PDDL/TestFiles/agricola-domain.pddl", "PDDL/TestFiles/agricola-prob01-undeclaredinitpredicate.pddl", 0, 1, 4)]
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
        [DataRow("PDDL/TestFiles/gripper-domain.pddl", "PDDL/TestFiles/gripper-prob01.pddl", 0, 0, 1)]
        [DataRow("PDDL/TestFiles/gripper-domain.pddl", "PDDL/TestFiles/gripper-prob01.pddl", 0, 0, 2)]
        [DataRow("PDDL/TestFiles/gripper-domain.pddl", "PDDL/TestFiles/gripper-prob01.pddl", 0, 1, 2)]
        [DataRow("PDDL/TestFiles/satellite-domain.pddl", "PDDL/TestFiles/satellite-prob01.pddl", 0, 0, 2)]
        [DataRow("PDDL/TestFiles/satellite-domain.pddl", "PDDL/TestFiles/satellite-prob01.pddl", 0, 1, 2)]
        [DataRow("PDDL/TestFiles/satellite-domain.pddl", "PDDL/TestFiles/satellite-prob01-duplicateobjects.pddl", 2, 1, 2)]
        [DataRow("PDDL/TestFiles/agricola-domain.pddl", "PDDL/TestFiles/agricola-prob01.pddl", 0, 0, 2)]
        [DataRow("PDDL/TestFiles/agricola-domain.pddl", "PDDL/TestFiles/agricola-prob01.pddl", 0, 0, 3)]
        [DataRow("PDDL/TestFiles/agricola-domain.pddl", "PDDL/TestFiles/agricola-prob01.pddl", 0, 0, 4)]
        [DataRow("PDDL/TestFiles/agricola-domain.pddl", "PDDL/TestFiles/agricola-prob01.pddl", 0, 0, 5)]
        [DataRow("PDDL/TestFiles/agricola-domain.pddl", "PDDL/TestFiles/agricola-prob01.pddl", 0, 1, 2)]
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
        [DataRow("PDDL/TestFiles/gripper-domain.pddl", "PDDL/TestFiles/gripper-prob01.pddl", 0, 0, 3)]
        [DataRow("PDDL/TestFiles/gripper-domain.pddl", "PDDL/TestFiles/gripper-prob01.pddl", 0, 0, 4)]
        [DataRow("PDDL/TestFiles/gripper-domain.pddl", "PDDL/TestFiles/gripper-prob01.pddl", 0, 0, 5)]
        [DataRow("PDDL/TestFiles/gripper-domain-unusedparamsaction.pddl", "PDDL/TestFiles/gripper-prob01.pddl", 0, 0, 3)]
        [DataRow("PDDL/TestFiles/gripper-domain-unusedparamsaction.pddl", "PDDL/TestFiles/gripper-prob01.pddl", 1, 0, 4)]
        [DataRow("PDDL/TestFiles/gripper-domain-unusedparamsaction.pddl", "PDDL/TestFiles/gripper-prob01.pddl", 2, 0, 5)]
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
        [DataRow("PDDL/TestFiles/satellite-domain.pddl", "PDDL/TestFiles/satellite-prob01.pddl", 0, 0, 3)]
        [DataRow("PDDL/TestFiles/satellite-domain.pddl", "PDDL/TestFiles/satellite-prob01.pddl", 0, 0, 4)]
        [DataRow("PDDL/TestFiles/satellite-domain.pddl", "PDDL/TestFiles/satellite-prob01.pddl", 0, 0, 5)]
        [DataRow("PDDL/TestFiles/satellite-domain.pddl", "PDDL/TestFiles/satellite-prob01.pddl", 0, 0, 6)]
        [DataRow("PDDL/TestFiles/satellite-domain.pddl", "PDDL/TestFiles/satellite-prob01.pddl", 0, 0, 7)]
        [DataRow("PDDL/TestFiles/satellite-domain-undeclaredparameter.pddl", "PDDL/TestFiles/satellite-prob01.pddl", 0, 0, 3)]
        [DataRow("PDDL/TestFiles/satellite-domain-undeclaredparameter.pddl", "PDDL/TestFiles/satellite-prob01.pddl", 1, 0, 4)]
        [DataRow("PDDL/TestFiles/satellite-domain-undeclaredparameter.pddl", "PDDL/TestFiles/satellite-prob01.pddl", 0, 0, 5)]
        [DataRow("PDDL/TestFiles/satellite-domain-undeclaredparameter.pddl", "PDDL/TestFiles/satellite-prob01.pddl", 0, 0, 6)]
        [DataRow("PDDL/TestFiles/satellite-domain-undeclaredparameter.pddl", "PDDL/TestFiles/satellite-prob01.pddl", 0, 0, 7)]
        [DataRow("PDDL/TestFiles/gripper-domain.pddl", "PDDL/TestFiles/gripper-prob01.pddl", 0, 0, 3)]
        [DataRow("PDDL/TestFiles/gripper-domain.pddl", "PDDL/TestFiles/gripper-prob01.pddl", 0, 0, 4)]
        [DataRow("PDDL/TestFiles/gripper-domain.pddl", "PDDL/TestFiles/gripper-prob01.pddl", 0, 0, 5)]
        [DataRow("PDDL/TestFiles/gripper-domain-undeclaredparameter.pddl", "PDDL/TestFiles/gripper-prob01.pddl", 0, 0, 3)]
        [DataRow("PDDL/TestFiles/gripper-domain-undeclaredparameter.pddl", "PDDL/TestFiles/gripper-prob01.pddl", 1, 0, 4)]
        [DataRow("PDDL/TestFiles/gripper-domain-undeclaredparameter.pddl", "PDDL/TestFiles/gripper-prob01.pddl", 0, 0, 5)]
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
