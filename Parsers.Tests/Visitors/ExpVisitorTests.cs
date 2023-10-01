using PDDLSharp.ASTGenerators;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.AST;
using PDDLSharp.Models.Domain;
using PDDLSharp.Models.Expressions;
using PDDLSharp.Parsers.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Parsers.Tests.Visitors
{
    [TestClass]
    public class ExpVisitorTests
    {
        private ASTNode GetParsed(string toParse)
        {
            IErrorListener listener = new ErrorListener();
            IGenerator parser = new ASTGenerator(listener);
            return parser.Generate(toParse);
        }

        [TestMethod]
        [DataRow("(and ())", typeof(AndExp))]
        [DataRow("(when (and ()) (and ()))", typeof(WhenExp))]
        [DataRow("(forall (pred) (and ()))", typeof(ForAllExp))]
        [DataRow("(exists (?a) (and ()))", typeof(ExistsExp))]
        [DataRow("(imply (a) (and ()))", typeof(ImplyExp))]
        [DataRow("(at 5 (a))", typeof(TimedLiteralExp))]
        [DataRow("10", typeof(LiteralExp))]
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
            var node = GetParsed(toParse);
            

            // ACT
            var exp = new ParserVisitor(null).VisitExp(node, null);

            // ASSERT
            Assert.IsInstanceOfType(exp, expectedType);
        }

        [TestMethod]
        [DataRow("59")]
        [DataRow("-1")]
        [DataRow("-1       ")]
        [DataRow("        -1       ")]
        public void Can_ParseLiteralExpNode(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            

            // ACT
            IExp? exp = new ParserVisitor(null).TryVisitLiteralNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(LiteralExp));
        }

        [TestMethod]
        [DataRow("(at 5 (a))")]
        [DataRow("(at 500 (pred a))")]
        [DataRow("(at -1 (pred a q))")]
        public void Can_ParseTimedLiteralExpNode(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            

            // ACT
            IExp? exp = new ParserVisitor(null).TryVisitTimedLiteralNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(TimedLiteralExp));
        }

        [TestMethod]
        [DataRow("(at 5 (a))", 5)]
        [DataRow("(at 5    (a))", 5)]
        [DataRow("(at 500 (pred a))", 500)]
        [DataRow("(at        500 (pred a) )", 500)]
        [DataRow("(at -1 (pred a q))", -1)]
        public void Can_ParseTimedLiteralExpNode_CorrectValue(string toParse, int expected)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            
            // ACT
            IExp? exp = new ParserVisitor(null).TryVisitTimedLiteralNode(node, null);

            // ASSERT
            Assert.AreEqual(expected, (exp as TimedLiteralExp).Value);
        }

        [TestMethod]
        [DataRow("(imply (a) (b))")]
        [DataRow("(imply (a) (and ()))")]
        [DataRow("(imply (and (a)) (and ()))")]
        public void Can_ParseImplyNode(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            

            // ACT
            IExp? exp = new ParserVisitor(null).TryVisitImplyNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(ImplyExp));
        }

        [TestMethod]
        [DataRow("(imply)")]
        [DataRow("(imply ())")]
        [DataRow("(imply () () ())")]
        [DataRow("(imply () () () ())")]
        public void Cant_ParseImplyNode_IfNotTwoChildren(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IExp? exp = new ParserVisitor(listener).TryVisitImplyNode(node, null);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("(imply (a) a (b))")]
        [DataRow("(imply (a) (and ()) ab)")]
        [DataRow("(imply s (and (a)) (and ()))")]
        public void Cant_ParseImplyNode_IfContainsStrayCharacters(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IExp? exp = new ParserVisitor(listener).TryVisitImplyNode(node, null);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("(exists (?a) (and ()))")]
        [DataRow("(exists (?a - type) (and ()))")]
        [DataRow("(exists (?a - type ?b - type2) (and ()))")]
        public void Can_ParseExistsNode(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            

            // ACT
            IExp? exp = new ParserVisitor(null).TryVisitExistsNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(ExistsExp));
        }

        [TestMethod]
        [DataRow("(exists ())")]
        [DataRow("(exists () () ())")]
        public void Cant_ParseExistsNode_IfNotTwoChildren(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IExp? exp = new ParserVisitor(listener).TryVisitExistsNode(node, null);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("(exists (?a) a (and ()))")]
        [DataRow("(exists (?a - type) abccc (and ()))")]
        [DataRow("(exists aafasf (?a - type ?b - type2) (and ()))")]
        public void Cant_ParseExistsNode_IfContainsStrayCharacters(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IExp? exp = new ParserVisitor(listener).TryVisitExistsNode(node, null);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("(forall (and ()) (and ()))")]
        [DataRow("(forall (and (a)) (and (b)))")]
        [DataRow("(forall (and (ab) (a)) (and ()))")]
        public void Can_ParseForAllNode(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            

            // ACT
            IExp? exp = new ParserVisitor(null).TryVisitForAllNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(ForAllExp));
        }

        [TestMethod]
        [DataRow("(forall)")]
        [DataRow("(forall  ())")]
        [DataRow("(forall  () () ())")]
        public void Cant_ParseForAllNode_IfNotTwoChildren(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IExp? exp = new ParserVisitor(listener).TryVisitForAllNode(node, null);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("(forall a (and ()) (and ()))")]
        [DataRow("(forall a (and ())     a (and ()))")]
        [DataRow("(forall a (and ())     a (and ()))q")]
        [DataRow("(forall (and ())(and ()))q")]
        public void Cant_ParseForAllNode_IfContainsStrayCharacters(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IExp? exp = new ParserVisitor(listener).TryVisitForAllNode(node, null);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("(when (and ()) (and ()))")]
        [DataRow("(when (and (a)) (and (b)))")]
        [DataRow("(when (and (ab) (a)) (and ()))")]
        public void Can_ParseWhenNode(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            

            // ACT
            IExp? exp = new ParserVisitor(null).TryVisitWhenNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(WhenExp));
        }

        [TestMethod]
        [DataRow("(when)")]
        [DataRow("(when  ())")]
        public void Cant_ParseWhenNode_IfNotTwoChildren(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IExp? exp = new ParserVisitor(listener).TryVisitWhenNode(node, null);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("(when a (and ()) (and ()))")]
        [DataRow("(when a (and ())     a (and ()))")]
        [DataRow("(when a (and ())     a (and ()))q")]
        [DataRow("(when (and ())(and ()))q")]
        public void Cant_ParseWhenNode_IfContainsStrayCharacters(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IExp? exp = new ParserVisitor(listener).TryVisitWhenNode(node, null);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("(and ())")]
        [DataRow("(and () () ())")]
        [DataRow("(and (abba) (aaa) (qwrer))")]
        public void Can_ParseAndNode(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            

            // ACT
            IExp? exp = new ParserVisitor(null).TryVisitAndNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(AndExp));
        }

        //[TestMethod]
        //[DataRow("(and)")]
        //[DataRow("(and         )")]
        //public void Cant_ParseAndNode_IfNoChildren(string toParse)
        //{
        //    // ARRANGE
        //    var node = GetParsed(toParse);
        //    
        //    IErrorListener listener = new ErrorListener();
        //    listener.ThrowIfTypeAbove = ParseErrorType.Error;

        //    // ACT
        //    IExp? exp = new ParserVisitor(listener).TryVisitAndNode(node, null);

        //    // ASSERT
        //    Assert.IsTrue(listener.Errors.Count > 0);
        //}

        [TestMethod]
        [DataRow("(and () q)")]
        [DataRow("(and       ()   b  )")]
        [DataRow("(and  () qqfa     ()   b  )")]
        public void Cant_ParseAndNode_IfContainsStrayCharacters(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IExp? exp = new ParserVisitor(listener).TryVisitAndNode(node, null);

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
            var node = GetParsed(toParse);
            

            // ACT
            IExp? exp = new ParserVisitor(null).TryVisitNumericNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(NumericExp));
        }

        [TestMethod]
        [DataRow("(or () ())")]
        [DataRow("(or (abcas) (qwefas))")]
        public void Can_ParseOrNode(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            

            // ACT
            IExp? exp = new ParserVisitor(null).TryVisitOrNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(OrExp));
        }

        [TestMethod]
        [DataRow("(or () a ())")]
        [DataRow("(or () a () bafras)")]
        [DataRow("(or  aaa()           a () bafras)")]
        public void Cant_ParseOrNode_IfContainsStrayCharacters(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IExp? exp = new ParserVisitor(listener).TryVisitOrNode(node, null);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("(not ())")]
        [DataRow("(not (abcas))")]
        public void Can_ParseNotNode(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            

            // ACT
            IExp? exp = new ParserVisitor(null).TryVisitNotNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(NotExp));
        }

        [TestMethod]
        [DataRow("(not)")]
        [DataRow("(not            )")]
        public void Cant_ParseNotNode_IfNoChild(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IExp? exp = new ParserVisitor(listener).TryVisitNotNode(node, null);

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
            var node = GetParsed(toParse);
            
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IExp? exp = new ParserVisitor(listener).TryVisitNotNode(node, null);

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
            var node = GetParsed(toParse);
            

            // ACT
            IExp? exp = new ParserVisitor(null).TryVisitPredicateNode(node, null);

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
            var node = GetParsed(toParse);
            

            // ACT
            IExp? exp = new ParserVisitor(null).TryVisitPredicateNode(node, null);

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
            var node = GetParsed(toParse);
            

            // ACT
            IExp? exp = new ParserVisitor(null).TryVisitPredicateNode(node, null);

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
        [DataRow("(pred ?a)", "object")]
        [DataRow("(pred ?a - type)", "type")]
        [DataRow("(pred ?a ?b - type)", "type", "type")]
        [DataRow("(pred ?a - type ?b - type)", "type", "type")]
        [DataRow("(pred ?a - type2 ?b - type)", "type2", "type")]
        [DataRow("(pred ?a - type2 ?c ?b - type)", "type2", "type", "type")]
        public void Can_ParsePredicateNode_CorrectParameterTypeNames(string toParse, params string[] expected)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            

            // ACT
            IExp? exp = new ParserVisitor(null).TryVisitPredicateNode(node, null);

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
            var node = GetParsed(toParse);
            

            // ACT
            IExp? exp = new ParserVisitor(null).TryVisitNameNode(node, null);

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
            var node = GetParsed(toParse);
            

            // ACT
            IExp? exp = new ParserVisitor(null).TryVisitNameNode(node, null);

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
            var node = GetParsed(toParse);
            

            // ACT
            IExp? exp = new ParserVisitor(null).TryVisitNameNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(NameExp));
            if (exp is NameExp name)
                Assert.AreEqual(expected, name.Type.Name);
        }
    }
}
