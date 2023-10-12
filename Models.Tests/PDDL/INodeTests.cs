using PDDLSharp;
using PDDLSharp.ASTGenerators;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.Tests;
using PDDLSharp.Models.Tests.PDDL;
using PDDLSharp.Parsers.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PDDLSharp.Models.Tests.PDDL
{
    [TestClass]
    public class INodeTests
    {
        [TestMethod]
        [DataRow("(define (domain a))", "a", 1)]
        [DataRow("(define (domain a) (:requirements a))", "a", 2)]
        [DataRow("(define (:action a :parameters (?a) :precondition (a) :effect (a)))", "?a", 1)]
        [DataRow("(define (:action b :parameters (?a) :precondition (a) :effect (a)))", "a", 2)]
        public void Can_FindNames_Domain(string toParse, string targetName, int expectedCount)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IGenerator astParser = new ASTGenerator(listener);
            var ast = astParser.Generate(toParse);
            Assert.IsNotNull(ast);
            DomainDecl? decl = new ParserVisitor(listener).TryVisitAs<DomainDecl>(ast, null) as DomainDecl;
            Assert.IsNotNull(decl);

            // ACT
            var found = decl.FindNames(targetName);

            // ASSERT
            Assert.IsTrue(found.Count == expectedCount);
            foreach (var item in found)
                Assert.AreEqual(item.Name, targetName);
        }

        [TestMethod]
        [DataRow("(define (domain a) (:requirements a) (:action a :parameters (?a) :precondition (pred a) :effect (pred a)))", nameof(NameExp), 4)]
        [DataRow("(define (domain a) (:requirements a) (:action a :parameters (?a) :precondition (pred a) :effect (pred a)))", nameof(PredicateExp), 2)]
        [DataRow("(define (domain a) (:requirements a) (:action a :parameters (?a) :precondition (pred a) :effect (pred a)))", nameof(ActionDecl), 1)]
        [DataRow("(define (domain a) (:requirements a) (:action a :parameters (?a) :precondition (pred a) :effect (pred a)))", nameof(RequirementsDecl), 1)]
        public void Can_FindTypes_Domain(string toParse, string targetType, int expectedCount)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IGenerator astParser = new ASTGenerator(listener);
            var ast = astParser.Generate(toParse);
            Assert.IsNotNull(ast);
            DomainDecl? decl = new ParserVisitor(listener).TryVisitAs<DomainDecl>(ast, null) as DomainDecl;
            Assert.IsNotNull(decl);

            // ACT
            // ASSERT
            switch (targetType)
            {
                case "NameExp": FindTypesTest<NameExp>(decl, expectedCount); break;
                case "PredicateExp": FindTypesTest<PredicateExp>(decl, expectedCount); break;
                case "ActionDecl": FindTypesTest<ActionDecl>(decl, expectedCount); break;
                case "RequirementsDecl": FindTypesTest<RequirementsDecl>(decl, expectedCount); break;
                default: Assert.Fail(); break;
            }
        }

        [TestMethod]
        [DataRow("(define (problem a))", "a", 1)]
        [DataRow("(define (problem a) (:domain b))", "a", 1)]
        [DataRow("(define (problem a) (:domain a))", "a", 2)]
        [DataRow("(define (problem a) (:domain a) (:objects a - type))", "a", 3)]
        public void Can_FindNames_Problem(string toParse, string targetName, int expectedCount)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IGenerator astParser = new ASTGenerator(listener);
            var ast = astParser.Generate(toParse);
            Assert.IsNotNull(ast);
            ProblemDecl? decl = new ParserVisitor(listener).TryVisitAs<ProblemDecl>(ast, null) as ProblemDecl;
            Assert.IsNotNull(decl);

            // ACT
            var found = decl.FindNames(targetName);

            // ASSERT
            Assert.IsTrue(found.Count == expectedCount);
            foreach (var item in found)
                Assert.AreEqual(item.Name, targetName);
        }

        [TestMethod]
        [DataRow("(define (problem a) (:objects a b c - q))", nameof(NameExp), 3)]
        [DataRow("(define (problem a) (:objects a b c - q))", nameof(TypeExp), 3)]
        [DataRow("(define (problem a) (:init (pred a) (a p)))", nameof(PredicateExp), 2)]
        [DataRow("(define (problem a) (:init (pred a) (a p)) (:goal (pred a)))", nameof(PredicateExp), 3)]
        public void Can_FindTypes_Problem(string toParse, string targetType, int expectedCount)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IGenerator astParser = new ASTGenerator(listener);
            var ast = astParser.Generate(toParse);
            Assert.IsNotNull(ast);
            ProblemDecl? decl = new ParserVisitor(listener).TryVisitAs<ProblemDecl>(ast, null) as ProblemDecl;
            Assert.IsNotNull(decl);

            // ACT
            // ASSERT
            switch (targetType)
            {
                case "NameExp": FindTypesTest<NameExp>(decl, expectedCount); break;
                case "TypeExp": FindTypesTest<TypeExp>(decl, expectedCount); break;
                case "PredicateExp": FindTypesTest<PredicateExp>(decl, expectedCount); break;
                default: Assert.Fail(); break;
            }
        }

        private void FindTypesTest<T>(INode decl, int expectedCount)
        {
            // ACT
            var found = decl.FindTypes<T>();

            // ASSERT
            Assert.AreEqual(expectedCount, found.Count);
        }
    }
}
