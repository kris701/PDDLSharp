using PDDLSharp.ASTGenerators;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.AST;
using PDDLSharp.Models.Domain;
using PDDLSharp.Models.Problem;
using PDDLSharp.Parsers.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PDDLSharp.Contextualisers.Tests
{
    [TestClass]
    public class PDDLProblemDeclContextualiserTests
    {
        [TestMethod]
        [DataRow("(define (:objects a) (:init (pred ?a) (pred2 ?a)) (:goal (not (?a)))", "a", "")]
        [DataRow("(define (:objects a - type) (:init (pred ?a) (pred2 ?a)) (:goal (not (?a)))", "a", "type")]
        [DataRow("(define (:objects a - q) (:init (pred ?a) (pred2 ?a)) (:goal (not (?a)))", "a", "q")]
        public void Can_DecorateObjectReferencesWithTypes(string toParse, string argName, string expectedType)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            ProblemDecl? decl = new ParserVisitor(listener).TryVisitAs<ProblemDecl>(node, null) as ProblemDecl;
            Assert.IsNotNull(decl);

            IContextualiser<ProblemDecl> contextualiser = new PDDLProblemDeclContextualiser(listener);

            // ACT
            contextualiser.Contexturalise(decl);

            // ASSERT
            foreach(var init in decl.Init.Predicates)
                Assert.IsTrue(ContextualiserTestsHelpers.AreAllNameExpOfTypeOrSubType(init, argName, expectedType));
            Assert.IsTrue(ContextualiserTestsHelpers.AreAllNameExpOfTypeOrSubType(decl.Goal.GoalExp, argName, expectedType));
        }
    }
}
