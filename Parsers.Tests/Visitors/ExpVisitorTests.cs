using PDDL.ASTGenerator;
using PDDL.ErrorListeners;
using PDDL.Models;
using PDDL.Models.AST;
using PDDL.Models.Domain;
using PDDL.Models.Expressions;
using PDDL.Parsers.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDL.Parsers.Tests.Visitors
{
    [TestClass]
    public class ExpVisitorTests
    {
        [TestMethod]
        [DataRow("(and ())", typeof(AndExp))]
        [DataRow("(not (aaabsbdsb))", typeof(NotExp))]
        [DataRow("(or (aaa) (bbb))", typeof(OrExp))]
        [DataRow("(pred)", typeof(PredicateExp))]
        [DataRow("(pred ?a)", typeof(PredicateExp))]
        [DataRow("(increase (pred ?a) 10)", typeof(NumericExp))]
        [DataRow("name=t=type", typeof(NameExp))]
        [DataRow("name", typeof(NameExp))]
        public void Can_VisitGeneral(string toParse, Type expectedType)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            var exp = new ExpVisitor().Visit(node, null, null);

            // ASSERT
            Assert.IsInstanceOfType(exp, expectedType);
        }

        [TestMethod]
        [DataRow("(and ())")]
        [DataRow("(and () () ())")]
        [DataRow("(and (abba) (aaa) (qwrer))")]
        public void Can_ParseAndNode(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IExp exp;
            new ExpVisitor().TryVisitAndNode(node, null, null, out exp);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(AndExp));
        }

        [TestMethod]
        [DataRow("(and)")]
        [DataRow("(and         )")]
        public void Cant_ParseAndNode_IfNoChildren(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IExp exp;
            new ExpVisitor().TryVisitAndNode(node, null, listener, out exp);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("(and () q)")]
        [DataRow("(and       ()   b  )")]
        [DataRow("(and  () qqfa     ()   b  )")]
        public void Cant_ParseAndNode_IfContainsStrayCharacters(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IExp exp;
            new ExpVisitor().TryVisitAndNode(node, null, listener, out exp);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("(increase (pred ?a) 10)")]
        [DataRow("(increase (pred ?a) (pred ?b))")]
        [DataRow("(decrease (pred ?a) 10)")]
        [DataRow("(decrease (pred ?a) (pred ?b))")]
        [DataRow("(assign (pred ?a) 10)")]
        [DataRow("(assign (pred ?a) (pred ?b))")]
        [DataRow("(scale-up (pred ?a) 10)")]
        [DataRow("(scale-up (pred ?a) (pred ?b))")]
        [DataRow("(scale-down (pred ?a) 10)")]
        [DataRow("(scale-down (pred ?a) (pred ?b))")]
        public void Can_ParseNumericExpNode(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IExp exp;
            new ExpVisitor().TryVisitNumericNode(node, null, null, out exp);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(NumericExp));
        }

        [TestMethod]
        [DataRow("(or () ())")]
        [DataRow("(or (abcas) (qwefas))")]
        public void Can_ParseOrNode(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IExp exp;
            new ExpVisitor().TryVisitOrNode(node, null, null, out exp);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(OrExp));
        }

        [TestMethod]
        [DataRow("(or () () ())")]
        [DataRow("(or ())")]
        [DataRow("(or)")]
        [DataRow("(or (abcas) () (qwefas))")]
        public void Cant_ParseOrNode_IfNotExactChildCount(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IExp exp;
            new ExpVisitor().TryVisitOrNode(node, null, listener, out exp);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("(or () a ())")]
        [DataRow("(or () a () bafras)")]
        [DataRow("(or  aaa()           a () bafras)")]
        public void Cant_ParseOrNode_IfContainsStrayCharacters(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IExp exp;
            new ExpVisitor().TryVisitOrNode(node, null, listener, out exp);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("(not ())")]
        [DataRow("(not (abcas))")]
        public void Can_ParseNotNode(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IExp exp;
            new ExpVisitor().TryVisitNotNode(node, null, null, out exp);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(NotExp));
        }

        [TestMethod]
        [DataRow("(not)")]
        [DataRow("(not            )")]
        public void Cant_ParseNotNode_IfNoChild(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IExp exp;
            new ExpVisitor().TryVisitNotNode(node, null, listener, out exp);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("(not () a)")]
        [DataRow("(not     a   ()       )")]
        [DataRow("(not     a   ()     dsgsdgdsg  )")]
        public void Cant_ParseNotNode_IfContainsStrayCharacters(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IExp exp;
            new ExpVisitor().TryVisitNotNode(node, null, listener, out exp);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("(pred)")]
        [DataRow("(pred ?a)")]
        [DataRow("(pred ?a ?b)")]
        public void Can_ParsePredicateNode(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IExp exp;
            new ExpVisitor().TryVisitPredicateNode(node, null, null, out exp);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(PredicateExp));
        }

        [TestMethod]
        [DataRow("(pred)", "pred")]
        [DataRow("(pred ?a)", "pred")]
        [DataRow("(pred ?a ?b)", "pred")]
        [DataRow("(q ?a ?b)", "q")]
        public void Can_ParsePredicateNode_CorrectName(string toParse, string expected)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IExp exp;
            new ExpVisitor().TryVisitPredicateNode(node, null, null, out exp);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(PredicateExp));
            if (exp is PredicateExp pred)
                Assert.AreEqual(expected, pred.Name);
        }

        [TestMethod]
        [DataRow("(pred ?a)", "?a")]
        [DataRow("(pred ?a ?b)", "?a", "?b")]
        [DataRow("(q ?a ?long)", "?a", "?long")]
        public void Can_ParsePredicateNode_CorrectParameterNames(string toParse, params string[] expected)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IExp exp;
            new ExpVisitor().TryVisitPredicateNode(node, null, null, out exp);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(PredicateExp));
            if (exp is PredicateExp pred)
            {
                Assert.AreEqual(expected.Length, pred.Arguments.Count);
                for (int i = 0; i < expected.Length; i++)
                    Assert.AreEqual(expected[i], pred.Arguments[i].Name);
            }
        }

        [TestMethod]
        [DataRow("(pred ?a)", "")]
        [DataRow("(pred ?a - type)", "type")]
        [DataRow("(pred ?a ?b - type)", "type", "type")]
        [DataRow("(pred ?a - type ?b - type)", "type", "type")]
        [DataRow("(pred ?a - type2 ?b - type)", "type2", "type")]
        [DataRow("(pred ?a - type2 ?c ?b - type)", "type2", "type", "type")]
        public void Can_ParsePredicateNode_CorrectParameterTypeNames(string toParse, params string[] expected)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IExp exp;
            new ExpVisitor().TryVisitPredicateNode(node, null, null, out exp);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(PredicateExp));
            if (exp is PredicateExp pred)
            {
                Assert.AreEqual(expected.Length, pred.Arguments.Count);
                for (int i = 0; i < expected.Length; i++)
                    Assert.AreEqual(expected[i], pred.Arguments[i].Type.Name);
            }
        }

        [TestMethod]
        [DataRow("(name)")]
        [DataRow("(name - type)")]
        public void Can_ParseNameNode(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IExp exp;
            new ExpVisitor().TryVisitNameNode(node, null, null, out exp);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(NameExp));
        }

        [TestMethod]
        [DataRow("(name)", "name")]
        [DataRow("(name - type)", "name")]
        [DataRow("(a - type)", "a")]
        public void Can_ParseNameNode_CorrectName(string toParse, string expected)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IExp exp;
            new ExpVisitor().TryVisitNameNode(node, null, null, out exp);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(NameExp));
            if (exp is NameExp name)
                Assert.AreEqual(expected, name.Name);
        }

        [TestMethod]
        [DataRow("(name)", "")]
        [DataRow("(name - type)", "type")]
        [DataRow("(a - type)", "type")]
        public void Can_ParseNameNode_CorrectTypeName(string toParse, string expected)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IExp exp;
            new ExpVisitor().TryVisitNameNode(node, null, null, out exp);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(NameExp));
            if (exp is NameExp name)
                Assert.AreEqual(expected, name.Type.Name);
        }
    }
}
