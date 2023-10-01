using PDDLSharp.ASTGenerators;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.AST;
using PDDLSharp.Models.Domain;
using PDDLSharp.Parsers.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Parsers.Tests.Visitors
{
    [TestClass]
    public class DomainVisitorTests
    {
        [TestMethod]
        [DataRow("(define (domain a))", typeof(DomainDecl))]
        [DataRow("(domain abc)", typeof(DomainNameDecl))]
        [DataRow("(:requirements abc)", typeof(RequirementsDecl))]
        [DataRow("(:extends :abc :other)", typeof(ExtendsDecl))]
        [DataRow("(:predicates (b) (c ?d))", typeof(PredicatesDecl))]
        [DataRow("(:functions (b) (c ?d))", typeof(FunctionsDecl))]
        [DataRow("(:constants a - b)", typeof(ConstantsDecl))]
        [DataRow("(:types a - b \n c - d)", typeof(TypesDecl))]
        [DataRow("(:timeless (a - b))", typeof(TimelessDecl))]
        [DataRow("(:action name :parameters (a) :precondition (not (a)) :effect (a))", typeof(ActionDecl))]
        [DataRow("(:durative-action name :parameters (a) :duration (= ?duration 10) :condition (not (a)) :effect (a))", typeof(DurativeActionDecl))]
        [DataRow("(:axiom :vars (a) :context (not (a)) :implies (a))", typeof(AxiomDecl))]
        [DataRow("(:derived (a) (not (b)))", typeof(DerivedDecl))]
        public void Can_VisitGeneral(string toParse, Type expectedType)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);

            // ACT
            IDecl? decl = new ParserVisitor(null).VisitDomain(node, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, expectedType);
        }

        [TestMethod]
        [DataRow("(define)")]
        [DataRow("(define (domain a))")]
        public void Can_ParseDomainNode(string toParse)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);

            // ACT
            IDecl? decl = new ParserVisitor(null).TryVisitDomainDeclNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(DomainDecl));
        }

        [TestMethod]
        [DataRow("(define abava)")]
        [DataRow("(define a)")]
        public void Cant_VisitProblemDecl_IfContainsStrayCharacters(string toParse)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IDecl? decl = new ParserVisitor(listener).TryVisitDomainDeclNode(node, null);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("(domain abc)")]
        [DataRow("(domain   abc)")]
        [DataRow("(domain   aasdafabc)")]
        public void Can_ParseDomainNameNode(string toParse)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);

            // ACT
            IDecl? decl = new ParserVisitor(null).TryVisitDomainNameNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(DomainNameDecl));
        }

        [TestMethod]
        [DataRow("(domain )")]
        [DataRow("(domain     )")]
        public void Cantt_VisitProblemDecl_IfContainsNoName(string toParse)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IDecl? decl = new ParserVisitor(listener).TryVisitDomainNameNode(node, null);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("(domain abc)")]
        public void Can_ParseDomainNameNode_CorrectName(string toParse)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);

            // ACT
            IDecl? decl = new ParserVisitor(null).TryVisitDomainNameNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(DomainNameDecl));
        }

        [TestMethod]
        [DataRow("(:requirements :abc :other)")]
        [DataRow("(:requirements :abc)")]
        [DataRow("(:requirements)")]
        public void Can_ParseRequirementsNode(string toParse)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);

            // ACT
            IDecl? decl = new ParserVisitor(null).TryVisitRequirementsNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(RequirementsDecl));
        }

        [TestMethod]
        [DataRow("(:requirements :abc :other)", ":abc", ":other")]
        [DataRow("(:requirements :abc)", ":abc")]
        public void Can_ParseRequirementsNode_CorrectNames(string toParse, params string[] expected)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);

            // ACT
            IDecl? decl = new ParserVisitor(null).TryVisitRequirementsNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(RequirementsDecl));
            if (decl is RequirementsDecl reqs)
            {
                Assert.AreEqual(expected.Length, reqs.Requirements.Count);
                for (int i = 0; i < reqs.Requirements.Count; i++)
                    Assert.AreEqual(expected[i], reqs.Requirements[i].Name);
            }
        }

        [TestMethod]
        [DataRow("(:extends :abc :other)")]
        [DataRow("(:extends :abc)")]
        [DataRow("(:extends)")]
        public void Can_ParseExtendsNode(string toParse)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);

            // ACT
            IDecl? decl = new ParserVisitor(null).TryVisitExtendsNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(ExtendsDecl));
        }

        [TestMethod]
        [DataRow("(:extends :abc :other)", ":abc", ":other")]
        [DataRow("(:extends :abc)", ":abc")]
        public void Can_ParseExtendsNode_CorrectNames(string toParse, params string[] expected)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);

            // ACT
            IDecl? decl = new ParserVisitor(null).TryVisitExtendsNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(ExtendsDecl));
            if (decl is ExtendsDecl exts)
            {
                Assert.AreEqual(expected.Length, exts.Extends.Count);
                for (int i = 0; i < exts.Extends.Count; i++)
                    Assert.AreEqual(expected[i], exts.Extends[i].Name);
            }
        }

        [TestMethod]
        [DataRow("(:types a - b \n c - d)")]
        [DataRow("(:types a - b)")]
        [DataRow("(:types)")]
        public void Can_ParseTypesNode(string toParse)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);

            // ACT
            IDecl? decl = new ParserVisitor(null).TryVisitTypesNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(TypesDecl));
        }

        [TestMethod]
        [DataRow("(:types a \n c)", "a", "c")]
        [DataRow("(:types a q \n e c)", "a", "q", "e", "c")]
        [DataRow("(:types a q e c)", "a", "q", "e", "c")]
        [DataRow("(:types a)", "a")]
        public void Can_ParseTypesNode_CorrectTypeNames(string toParse, params string[] expected)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);

            // ACT
            IDecl? decl = new ParserVisitor(null).TryVisitTypesNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(TypesDecl));
            if (decl is TypesDecl types)
            {
                Assert.AreEqual(expected.Length, types.Types.Count);
                for (int i = 0; i < types.Types.Count; i++)
                    Assert.AreEqual(expected[i], types.Types[i].Name);
            }
        }

        [TestMethod]
        [DataRow("(:types site material - object \n bricks cables windows - material)", "object", "object", "material", "material", "material")]
        [DataRow("(:types a - b \n c - d)", "b", "d")]
        [DataRow("(:types a q - b \n e c - d)", "b", "b", "d", "d")]
        [DataRow("(:types a - b)", "b")]
        public void Can_ParseTypesNode_CorrectInheritTypeNames(string toParse, params string[] expected)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);

            // ACT
            IDecl? decl = new ParserVisitor(null).TryVisitTypesNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(TypesDecl));
            if (decl is TypesDecl types)
            {
                Assert.AreEqual(expected.Length, types.Types.Count);
                for (int i = 0; i < types.Types.Count; i++)
                    Assert.IsTrue(types.Types[i].IsTypeOf(expected[i]));
            }
        }

        [TestMethod]
        [DataRow("(:types a - b \n b - object)", "b;object;", "object;")]
        [DataRow("(:types a - object \n b - a)", "object;", "a;object;")]
        [DataRow("(:types a - object \n b c - a)", "object;", "a;object;", "a;object;")]
        [DataRow("(:types a - object \n b c - a \n q)", "object;", "a;object;", "a;object;", ";")]
        [DataRow("(:types a - object \n b c - a \n q \n d - c)", "object;", "a;object;", "a;object;", "c;a;object;", "c;a;object;")]
        public void Can_ParseTypesNode_CorrectInheritSuperTypes(string toParse, params string[] expected)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);

            // ACT
            IDecl? decl = new ParserVisitor(null).TryVisitTypesNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(TypesDecl));
            if (decl is TypesDecl types)
            {
                Assert.AreEqual(expected.Length, types.Types.Count);
                for (int i = 0; i < types.Types.Count; i++)
                {
                    var str = "";
                    foreach (var supertype in types.Types[i].SuperTypes)
                        str += $"{supertype};";
                    Assert.AreEqual(expected[i], str);
                }
            }
        }

        [TestMethod]
        [DataRow("(:constants)")]
        [DataRow("(:constants a)")]
        [DataRow("(:constants a b)")]
        [DataRow("(:constants a - b)")]
        [DataRow("(:constants a - b other - type)")]
        public void Can_ParseConstantsNode(string toParse)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);

            // ACT
            IDecl? decl = new ParserVisitor(null).TryVisitConstantsNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(ConstantsDecl));
        }

        [TestMethod]
        [DataRow("(:constants a)", "a")]
        [DataRow("(:constants a b)", "a", "b")]
        [DataRow("(:constants a - b)", "a")]
        [DataRow("(:constants a - b other - type)", "a", "other")]
        public void Can_ParseConstantsNode_CorrectNames(string toParse, params string[] expected)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);

            // ACT
            IDecl? decl = new ParserVisitor(null).TryVisitConstantsNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(ConstantsDecl));
            if (decl is ConstantsDecl con)
            {
                Assert.AreEqual(expected.Length, con.Constants.Count);
                for (int i = 0; i < con.Constants.Count; i++)
                    Assert.AreEqual(expected[i], con.Constants[i].Name);
            }
        }

        [TestMethod]
        [DataRow("(:constants a - b)", "b")]
        [DataRow("(:constants a - b other - type)", "b", "type")]
        [DataRow("(:constants a - b other - type c)", "b", "type", "object")]
        public void Can_ParseConstantsNode_CorrectTypeNames(string toParse, params string[] expected)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);

            // ACT
            IDecl? decl = new ParserVisitor(null).TryVisitConstantsNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(ConstantsDecl));
            if (decl is ConstantsDecl con)
            {
                Assert.AreEqual(expected.Length, con.Constants.Count);
                for (int i = 0; i < con.Constants.Count; i++)
                    Assert.AreEqual(expected[i], con.Constants[i].Type.Name);
            }
        }

        [TestMethod]
        [DataRow("(:predicates)")]
        [DataRow("(:predicates (a))")]
        [DataRow("(:predicates (b) (c ?d))")]
        public void Can_ParsePredicatesNode(string toParse)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);

            // ACT
            IDecl? decl = new ParserVisitor(null).TryVisitPredicatesNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(PredicatesDecl));
        }

        [TestMethod]
        [DataRow("(:predicates (a))", "a")]
        [DataRow("(:predicates (b) (c ?d))", "b", "c")]
        public void Can_ParsePredicatesNode_CorrectName(string toParse, params string[] expecteds)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);

            // ACT
            IDecl? decl = new ParserVisitor(null).TryVisitPredicatesNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(PredicatesDecl));
            if (decl is PredicatesDecl preds)
            {
                Assert.AreEqual(preds.Predicates.Count, expecteds.Length);
                for (int i = 0; i < preds.Predicates.Count; i++)
                    Assert.AreEqual(expecteds[i], preds.Predicates[i].Name);
            }
        }

        [TestMethod]
        [DataRow("(:functions)")]
        [DataRow("(:functions (a))")]
        [DataRow("(:functions (b) (c ?d))")]
        public void Can_ParseFunctionsNode(string toParse)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);

            // ACT
            IDecl? decl = new ParserVisitor(null).TryVisitFunctionsNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(FunctionsDecl));
        }

        [TestMethod]
        [DataRow("(:functions (a))", "a")]
        [DataRow("(:functions (b) (c ?d))", "b", "c")]
        public void Can_ParseFunctionsNode_CorrectName(string toParse, params string[] expecteds)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);

            // ACT
            IDecl? decl = new ParserVisitor(null).TryVisitFunctionsNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(FunctionsDecl));
            if (decl is FunctionsDecl funcs)
            {
                Assert.AreEqual(funcs.Functions.Count, expecteds.Length);
                for (int i = 0; i < funcs.Functions.Count; i++)
                    Assert.AreEqual(expecteds[i], funcs.Functions[i].Name);
            }
        }

        [TestMethod]
        [DataRow("(:timeless)")]
        [DataRow("(:timeless (a))")]
        [DataRow("(:timeless (a ?q - b))")]
        [DataRow("(:timeless (a ?q - b) (c))")]
        public void Can_ParseTimelessNode(string toParse)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);

            // ACT
            IDecl? decl = new ParserVisitor(null).TryVisitTimelessNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(TimelessDecl));
        }

        [TestMethod]
        [DataRow("(:timeless (a))", "a")]
        [DataRow("(:timeless (a ?q - b))", "a")]
        [DataRow("(:timeless (a ?q - b) (c))", "a", "c")]
        [DataRow("(:timeless (a ?q - b) (c)    (d))", "a", "c", "d")]
        public void Can_ParseTimelessNode_CorrectNames(string toParse, params string[] expecteds)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);

            // ACT
            IDecl? decl = new ParserVisitor(null).TryVisitTimelessNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(TimelessDecl));
            if (decl is TimelessDecl timel)
            {
                Assert.AreEqual(expecteds.Length, timel.Items.Count);
                for (int i = 0; i < timel.Items.Count; i++)
                    Assert.AreEqual(expecteds[i], timel.Items[i].Name);
            }
        }

        [TestMethod]
        [DataRow("(:action name :parameters () :precondition () :effect ())")]
        [DataRow("(:action name :parameters (a) :precondition (not (a)) :effect (a))")]
        public void Can_ParseActionNode(string toParse)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);

            // ACT
            IDecl? decl = new ParserVisitor(null).TryVisitActionNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(ActionDecl));
        }

        [TestMethod]
        [DataRow("(:action name :parameters () :precondition () :effect ())", "name")]
        [DataRow("(:action othername :parameters (a) :precondition (not (a)) :effect (a))", "othername")]
        public void Can_ParseActionNode_CorrectActionName(string toParse, string expected)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);

            // ACT
            IDecl? decl = new ParserVisitor(null).TryVisitActionNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(ActionDecl));
            if (decl is ActionDecl act)
                Assert.AreEqual(expected, act.Name);
        }

        [TestMethod]
        [DataRow("(:action name () :precondition () :effect ())")]
        [DataRow("(:action name :parameters () () :effect ())")]
        [DataRow("(:action name :parameters () :precondition () ())")]
        [DataRow("(:action name () () ())")]

        public void Cant_ParseActionNode_IfMissingPrimaryElements(string toParse)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IDecl? decl = new ParserVisitor(listener).TryVisitActionNode(node, null);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("(:action  :parameters () :precondition () :effect ())")]
        [DataRow("(:action        :parameters () :precondition () :effect ())")]

        public void Cant_ParseActionNode_IfMissingName(string toParse)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IDecl? decl = new ParserVisitor(listener).TryVisitActionNode(node, null);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("(:durative-action name :parameters () :duration () :condition () :effect ())")]
        [DataRow("(:durative-action name :parameters (a) :duration (= ?duration 10) :condition (not (a)) :effect (a))")]
        public void Can_ParseDurativeActionNode(string toParse)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);

            // ACT
            IDecl? decl = new ParserVisitor(null).TryVisitDurativeActionNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(DurativeActionDecl));
        }

        [TestMethod]
        [DataRow("(:durative-action name :parameters () :duration () :condition () :effect ())", "name")]
        [DataRow("(:durative-action othername :parameters (a) :duration (= ?duration 10) :condition (not (a)) :effect (a))", "othername")]
        public void Can_ParseDurativeActionNode_CorrectActionName(string toParse, string expected)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);

            // ACT
            IDecl? decl = new ParserVisitor(null).TryVisitDurativeActionNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(DurativeActionDecl));
            if (decl is ActionDecl act)
                Assert.AreEqual(expected, act.Name);
        }

        [TestMethod]
        [DataRow("(:durative-action name :parameters () :condition () :effect ())")]
        [DataRow("(:durative-action name :parameters () :duration () :effect ())")]
        [DataRow("(:durative-action name :condition () () :effect ())")]
        [DataRow("(:durative-action name :condition () :precondition () ())")]
        [DataRow("(:durative-action name () () ())")]

        public void Cant_ParseDurativeActionNode_IfMissingPrimaryElements(string toParse)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IDecl? decl = new ParserVisitor(listener).TryVisitDurativeActionNode(node, null);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("(:durative-action  :parameters () :duration () :condition () :effect ())")]
        [DataRow("(:durative-action             :parameters (a) :duration (= ?duration 10) :condition (not (a)) :effect (a))")]

        public void Cant_ParseDurativeActionNode_IfMissingName(string toParse)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IDecl? decl = new ParserVisitor(listener).TryVisitDurativeActionNode(node, null);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("(:axiom :vars () :context () :implies ())")]
        [DataRow("(:axiom :vars (a) :context (not (a)) :implies (a))")]
        public void Can_ParseAxiomNode(string toParse)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);

            // ACT
            IDecl? decl = new ParserVisitor(null).TryVisitAxiomNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(AxiomDecl));
        }

        [TestMethod]
        [DataRow("(:axiom () :context () :implies ())")]
        [DataRow("(:axiom :vars () () :implies ())")]
        [DataRow("(:axiom :vars () :context () ())")]
        [DataRow("(:axiom () () ())")]
        public void Cant_ParseAxiomNode_IfMissingPrimaryElements(string toParse)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IDecl? decl = new ParserVisitor(listener).TryVisitAxiomNode(node, null);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("(:derived (a) (not (b)))")]
        [DataRow("(:derived (pred a - type) (not (b)))")]
        [DataRow("(:derived (pred a - type) (and (b) (c)))")]
        public void Can_ParseDerivedNode(string toParse)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);

            // ACT
            IDecl? decl = new ParserVisitor(null).TryVisitDerivedNode(node, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(DerivedDecl));
        }

        [TestMethod]
        [DataRow("(:derived ()")]
        [DataRow("(:derived () () ()")]
        public void Cant_ParseDerivedNode_IfNotTwoChildren(string toParse)
        {
            // ARRANGE
            IGenerator<ASTNode> parser = new ASTGenerator();
            var node = parser.Generate(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IDecl? decl = new ParserVisitor(listener).TryVisitDerivedNode(node, null);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }
    }
}
