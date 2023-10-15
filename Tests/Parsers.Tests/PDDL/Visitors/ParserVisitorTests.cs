using PDDLSharp;
using PDDLSharp.ASTGenerators;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.AST;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.Tests;
using PDDLSharp.Parsers.Tests.PDDL.Visitors;
using PDDLSharp.Parsers.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Parsers.Tests.PDDL.Visitors
{
    [TestClass]
    public class ParserVisitorTests
    {
        [TestMethod]
        [DataRow("test", "(test)", "test")]
        [DataRow("test    ", "(test    )", "test")]
        [DataRow("    test    ", "(    test    )", "test")]
        public void Can_Pass_If_DoesNotContainStrayCharacters(string innerText, string outerText, string targetName)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);
            var parser = new ParserVisitor(listener);
            var node = new ASTNode(0, 0, outerText, innerText);

            // ACT
            var result = parser.DoesNotContainStrayCharacters(node, targetName);

            // ASSERT
            Assert.IsTrue(result);
            Assert.IsTrue(listener.CountErrorsOfTypeOrAbove(ParseErrorType.Error) == 0);
        }

        [TestMethod]
        [DataRow("test a", "(test a)", "test")]
        [DataRow("abc test a", "(abc test a)", "test")]
        [DataRow("test           ()", "(test           ())", "test")]
        public void Cant_Pass_If_Not_DoesNotContainStrayCharacters(string innerText, string outerText, string targetName)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);
            var parser = new ParserVisitor(listener);
            var node = new ASTNode(0, 0, outerText, innerText);

            // ACT
            var result = parser.DoesNotContainStrayCharacters(node, targetName);

            // ASSERT
            Assert.IsFalse(result);
            Assert.IsTrue(listener.CountErrorsOfTypeOrAbove(ParseErrorType.Error) >= 1);
        }
    }
}
