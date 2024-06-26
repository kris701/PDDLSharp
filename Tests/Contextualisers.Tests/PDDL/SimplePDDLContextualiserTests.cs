﻿using PDDLSharp.ASTGenerators;
using PDDLSharp.ASTGenerators.PDDL;
using PDDLSharp.Contextualisers.PDDL;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Contextualisers.Tests.PDDL
{
    [TestClass]
    public class SimplePDDLContextualiserTests
    {
        #region Domain
        [TestMethod]
        [DataRow("(define (:action name :parameters (?a) :precondition (p a) :effect (p a)))", "?a", "")]
        [DataRow("(define (:action name :parameters (?a - type) :precondition (p a) :effect (p a)))", "?a", "type")]
        [DataRow("(define (:action name :parameters (?a - type) :precondition (not (p a)) :effect (p a)))", "?a", "type")]
        [DataRow("(define (:action name :parameters (?a - type) :precondition (not (p a)) :effect (and (p a))))", "?a", "type")]
        [DataRow("(define (:action name :parameters (?a - type) :precondition (not (p a)) :effect (or (p a) (not (p a)))))", "?a", "type")]
        public void Can_DecorateActionParameterReferencesWithType(string toParse, string argName, string expectedType)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            IGenerator parser = new PDDLASTGenerator(listener);
            var node = parser.Generate(toParse);
            DomainDecl? decl = new ParserVisitor(listener).TryVisitAs<DomainDecl>(node, null) as DomainDecl;
            Assert.IsNotNull(decl);

            IContextualiser contextualiser = new PDDLContextualiser(listener);

            // ACT
            contextualiser.Contexturalise(new PDDLDecl(decl, new ProblemDecl()));

            // ASSERT
            Assert.IsTrue(ContextualiserTestsHelpers.AreAllNameExpOfTypeOrSubType(decl.Actions[0].Preconditions, argName, expectedType));
            Assert.IsTrue(ContextualiserTestsHelpers.AreAllNameExpOfTypeOrSubType(decl.Actions[0].Effects, argName, expectedType));
        }

        [TestMethod]
        [DataRow("(define (:axiom :vars (?a) :context (p a) :implies (p a)))", "?a", "")]
        [DataRow("(define (:axiom :vars (?a - type) :context (p a) :implies (p a)))", "?a", "type")]
        [DataRow("(define (:axiom :vars (?a - type) :context (not (p a)) :implies (p a)))", "?a", "type")]
        [DataRow("(define (:axiom :vars (?a - type) :context (not (p a)) :implies (and (p a))))", "?a", "type")]
        [DataRow("(define (:axiom :vars (?a - type) :context (not (p a)) :implies (or (p a) (not (p a)))))", "?a", "type")]
        public void Can_DecorateAxiomParameterReferencesWithType(string toParse, string argName, string expectedType)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            IGenerator parser = new PDDLASTGenerator(listener);
            var node = parser.Generate(toParse);
            DomainDecl? decl = new ParserVisitor(listener).TryVisitAs<DomainDecl>(node, null) as DomainDecl;
            Assert.IsNotNull(decl);

            IContextualiser contextualiser = new PDDLContextualiser(listener);

            // ACT
            contextualiser.Contexturalise(new PDDLDecl(decl, new ProblemDecl()));

            // ASSERT
            Assert.IsTrue(ContextualiserTestsHelpers.AreAllNameExpOfTypeOrSubType(decl.Axioms[0].Context, argName, expectedType));
            Assert.IsTrue(ContextualiserTestsHelpers.AreAllNameExpOfTypeOrSubType(decl.Axioms[0].Implies, argName, expectedType));
        }

        [TestMethod]
        [DataRow("(define (:types type - supertype) (:axiom :vars (?a - type) :context (p a) :implies (p a)))", "?a", "supertype")]
        [DataRow("(define (:types type - other \n supertype - highersupertype) (:axiom :vars (?a - type) :context (p a) :implies (p a)))", "?a", "other")]
        [DataRow("(define (:types type - other \n supertype - type) (:axiom :vars (?a - type) :context (p a) :implies (p a)))", "?a", "other")]
        [DataRow("(define (:types type) (:axiom :vars (?a - type) :context (p a) :implies (p a)))", "?a", "")]
        public void Can_CanDecorateSubtypes(string toParse, string argName, string expectedSuperType)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            IGenerator parser = new PDDLASTGenerator(listener);
            var node = parser.Generate(toParse);
            DomainDecl? decl = new ParserVisitor(listener).TryVisitAs<DomainDecl>(node, null) as DomainDecl;
            Assert.IsNotNull(decl);

            IContextualiser contextualiser = new PDDLContextualiser(listener);

            // ACT
            contextualiser.Contexturalise(new PDDLDecl(decl, new ProblemDecl()));

            // ASSERT
            Assert.IsTrue(ContextualiserTestsHelpers.AreAllNameExpOfTypeOrSubType(decl.Axioms[0].Context, argName, expectedSuperType));
            Assert.IsTrue(ContextualiserTestsHelpers.AreAllNameExpOfTypeOrSubType(decl.Axioms[0].Implies, argName, expectedSuperType));
        }
        #endregion

        #region Problem
        [TestMethod]
        [DataRow("(define (:objects a) (:init (pred ?a) (pred2 ?a)) (:goal (not (?a))))", "a", "")]
        [DataRow("(define (:objects a - type) (:init (pred ?a) (pred2 ?a)) (:goal (not (?a))))", "a", "type")]
        [DataRow("(define (:objects a - q) (:init (pred ?a) (pred2 ?a)) (:goal (not (?a))))", "a", "q")]
        public void Can_DecorateObjectReferencesWithTypes(string toParse, string argName, string expectedType)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            IGenerator parser = new PDDLASTGenerator(listener);
            var node = parser.Generate(toParse);
            ProblemDecl? decl = new ParserVisitor(listener).TryVisitAs<ProblemDecl>(node, null) as ProblemDecl;
            Assert.IsNotNull(decl);

            IContextualiser contextualiser = new PDDLContextualiser(listener);

            // ACT
            contextualiser.Contexturalise(new PDDLDecl(new DomainDecl(), decl));

            // ASSERT
            foreach (var init in decl.Init.Predicates)
                Assert.IsTrue(ContextualiserTestsHelpers.AreAllNameExpOfTypeOrSubType(init, argName, expectedType));
            Assert.IsTrue(ContextualiserTestsHelpers.AreAllNameExpOfTypeOrSubType(decl.Goal.GoalExp, argName, expectedType));
        }
        #endregion
    }
}
