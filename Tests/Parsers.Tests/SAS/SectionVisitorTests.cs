using PDDLSharp.ASTGenerators;
using PDDLSharp.ASTGenerators.FastDownward.SAS;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.AST;
using PDDLSharp.Models.FastDownward.SAS;
using PDDLSharp.Models.FastDownward.SAS.Sections;
using PDDLSharp.Parsers.FastDownward.SAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Parsers.Tests.SAS
{
    [TestClass]
    public class SectionVisitorTests
    {
        private ASTNode GetParsed(string toParse)
        {
            IErrorListener listener = new ErrorListener();
            IGenerator parser = new SASASTGenerator(listener);
            return parser.Generate(toParse);
        }

        [TestMethod]
        [DataRow("begin_rule\n0\n0 0 0\nend_rule\n", typeof(AxiomDecl))]
        [DataRow("begin_operator\nop1\n0\n0\n0\nend_operator\n", typeof(OperatorDecl))]
        [DataRow("begin_goal\n0\nend_goal\n", typeof(GoalStateDecl))]
        [DataRow("begin_state\n0\nend_state\n", typeof(InitStateDecl))]
        [DataRow("begin_mutex_group\n0\nend_mutex_group\n", typeof(MutexDecl))]
        [DataRow("begin_variable\nvar0\n-1\n0\nend_variable\n", typeof(VariableDecl))]
        [DataRow("begin_metric\n0\nend_metric\n", typeof(MetricDecl))]
        [DataRow("begin_version\n3\nend_version\n", typeof(VersionDecl))]
        public void Can_VisitGeneral(string toParse, Type expectedType)
        {
            // ARRANGE
            var node = GetParsed(toParse);

            // ACT
            ISASNode? res = new SectionVisitor(null).VisitSections(node.Children[0]);

            // ASSERT
            Assert.IsInstanceOfType(res, expectedType);
        }

        #region Version Node

        [TestMethod]
        [DataRow("begin_version\n3\nend_version\n", 3)]
        [DataRow("begin_version\n2\nend_version\n", 2)]
        [DataRow("begin_version\n646437\nend_version\n", 646437)]
        public void Can_VisitVersionNode(string toParse, int expected)
        {
            // ARRANGE
            var node = GetParsed(toParse);

            // ACT
            ISASNode? res = new SectionVisitor(null).TryVisitAsVersion(node.Children[0]);

            // ASSERT
            Assert.IsInstanceOfType(res, typeof(VersionDecl));
            if (res is VersionDecl ver)
                Assert.AreEqual(expected, ver.Version);
        }

        [TestMethod]
        [DataRow("begin_version\na\nend_version\n")]
        [DataRow("begin_version\na 1\nend_version\n")]
        [DataRow("begin_version\nabbbbb\nend_version\n")]
        public void Cant_VisitVersionNode_IfNotANumber(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);

            // ACT
            ISASNode? res = new SectionVisitor(listener).TryVisitAsVersion(node.Children[0]);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("begin_version\nend_version\n")]
        [DataRow("begin_version\n\nend_version\n")]
        [DataRow("begin_version\n\n\nend_version\n")]
        [DataRow("begin_version\n     \nend_version\n")]
        public void Cant_VisitVersionNode_IfEmpty(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);

            // ACT
            ISASNode? res = new SectionVisitor(listener).TryVisitAsVersion(node.Children[0]);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        #endregion

        #region Metric Node

        [TestMethod]
        [DataRow("begin_metric\n1\nend_metric\n", true)]
        [DataRow("begin_metric\n0\nend_metric\n", false)]
        public void Can_VisitMetricNode(string toParse, bool expected)
        {
            // ARRANGE
            var node = GetParsed(toParse);

            // ACT
            ISASNode? res = new SectionVisitor(null).TryVisitAsMetric(node.Children[0]);

            // ASSERT
            Assert.IsInstanceOfType(res, typeof(MetricDecl));
            if (res is MetricDecl ver)
                Assert.AreEqual(expected, ver.IsUsingMetrics);
        }

        [TestMethod]
        [DataRow("begin_metric\n-1\nend_metric\n")]
        [DataRow("begin_metric\n2\nend_metric\n")]
        [DataRow("begin_metric\n63295\nend_metric\n")]
        public void Cant_VisitMetricNode_IfNot1or0(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);

            // ACT
            ISASNode? res = new SectionVisitor(listener).TryVisitAsMetric(node.Children[0]);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("begin_metric\nend_metric\n")]
        [DataRow("begin_metric\n\nend_metric\n")]
        [DataRow("begin_metric\n  \n \nend_metric\n")]
        public void Cant_VisitMetricNode_IfEmpty(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);

            // ACT
            ISASNode? res = new SectionVisitor(listener).TryVisitAsMetric(node.Children[0]);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        #endregion

        #region Variable Node

        [TestMethod]
        [DataRow("begin_variable\nvar1\n-1\n1\nAtom\nend_variable\n", "var1", -1, 1)]
        [DataRow("begin_variable\nvar1\n2\n1\nAtom\nend_variable\n", "var1", 2, 1)]
        [DataRow("begin_variable\nvar1\n2\n2\nAtom\nAtom2\nend_variable\n", "var1", 2, 2)]
        public void Can_VisitVariableNode(string toParse, string expectedName, int expectedAxiomLayer, int expectedSymbolicCount)
        {
            // ARRANGE
            var node = GetParsed(toParse);

            // ACT
            ISASNode? res = new SectionVisitor(null).TryVisitAsVariable(node.Children[0]);

            // ASSERT
            Assert.IsInstanceOfType(res, typeof(VariableDecl));
            if (res is VariableDecl ver)
            {
                Assert.AreEqual(expectedName, ver.VariableName);
                Assert.AreEqual(expectedAxiomLayer, ver.AxiomLayer);
                Assert.AreEqual(expectedSymbolicCount, ver.SymbolicNames.Count);
            }
        }

        [TestMethod]
        [DataRow("begin_variable\n-1\n1\nAtom\nend_variable\n")]
        [DataRow("begin_variable\n\n-1\n1\nAtom\nend_variable\n")]
        public void Cant_VisitVariableNode_IfNoNameGiven(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);

            // ACT
            ISASNode? res = new SectionVisitor(listener).TryVisitAsVariable(node.Children[0]);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("begin_variable\nvar1\n1\nAtom\nend_variable\n")]
        [DataRow("begin_variable\nvar1\n\n1\nAtom\nend_variable\n")]
        public void Cant_VisitVariableNode_IfAxiomLayerNotGiven(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);

            // ACT
            ISASNode? res = new SectionVisitor(listener).TryVisitAsVariable(node.Children[0]);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("begin_variable\nvar1\n-1\n1\nend_variable\n")]
        [DataRow("begin_variable\nvar1\n-1\n1\nAtom\nAtom2\nend_variable\n")]
        public void Cant_VisitVariableNode_SymbolicCountDontMatch(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);

            // ACT
            ISASNode? res = new SectionVisitor(listener).TryVisitAsVariable(node.Children[0]);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        #endregion

        #region Mutex Node

        [TestMethod]
        [DataRow("begin_mutex_group\n0\nend_mutex_group\n", 0)]
        [DataRow("begin_mutex_group\n1\n0 1\nend_mutex_group\n", 1)]
        [DataRow("begin_mutex_group\n2\n0 1\n0 2\nend_mutex_group\n", 2)]
        public void Can_VisitMutexNode(string toParse, int expected)
        {
            // ARRANGE
            var node = GetParsed(toParse);

            // ACT
            ISASNode? res = new SectionVisitor(null).TryVisitAsMutex(node.Children[0]);

            // ASSERT
            Assert.IsInstanceOfType(res, typeof(MutexDecl));
            if (res is MutexDecl ver)
                Assert.AreEqual(expected, ver.Group.Count);
        }

        [TestMethod]
        [DataRow("begin_mutex_group\nend_mutex_group\n")]
        [DataRow("begin_mutex_group\n\nend_mutex_group\n")]
        public void Cant_VisitMutexNode_IfMutexCountNotGiven(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);

            // ACT
            ISASNode? res = new SectionVisitor(listener).TryVisitAsMutex(node.Children[0]);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("begin_mutex_group\n0\n\nend_mutex_group\n")]
        [DataRow("begin_mutex_group\n0\n\n\nend_mutex_group\n")]
        public void Cant_VisitMutexNode_IfContainingEmptyLines(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);

            // ACT
            ISASNode? res = new SectionVisitor(listener).TryVisitAsMutex(node.Children[0]);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("begin_mutex_group\n0\n1 0\nend_mutex_group\n")]
        [DataRow("begin_mutex_group\n2\n1 0\nend_mutex_group\n")]
        [DataRow("begin_mutex_group\n50\n1 0\nend_mutex_group\n")]
        public void Cant_VisitMutexNode_IfFactCountNotMatch(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);

            // ACT
            ISASNode? res = new SectionVisitor(listener).TryVisitAsMutex(node.Children[0]);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        #endregion

        #region Init Node

        [TestMethod]
        [DataRow("begin_state\nend_state\n", 0)]
        [DataRow("begin_state\n0\nend_state\n", 1)]
        [DataRow("begin_state\n0\n2\nend_state\n", 2)]
        public void Can_VisitInitDeclNode(string toParse, int expected)
        {
            // ARRANGE
            var node = GetParsed(toParse);

            // ACT
            ISASNode? res = new SectionVisitor(null).TryVisitAsInitState(node.Children[0]);

            // ASSERT
            Assert.IsInstanceOfType(res, typeof(InitStateDecl));
            if (res is InitStateDecl ver)
                Assert.AreEqual(expected, ver.Inits.Count);
        }

        #endregion

        #region Goal Node

        [TestMethod]
        [DataRow("begin_goal\n0\nend_goal\n", 0)]
        [DataRow("begin_goal\n1\n0 1\nend_goal\n", 1)]
        [DataRow("begin_goal\n2\n0 1\n1 0\nend_goal\n", 2)]
        public void Can_VisitGoalDeclNode(string toParse, int expected)
        {
            // ARRANGE
            var node = GetParsed(toParse);

            // ACT
            ISASNode? res = new SectionVisitor(null).TryVisitAsGoalState(node.Children[0]);

            // ASSERT
            Assert.IsInstanceOfType(res, typeof(GoalStateDecl));
            if (res is GoalStateDecl ver)
                Assert.AreEqual(expected, ver.Goals.Count);
        }

        [TestMethod]
        [DataRow("begin_goal\nend_goal\n")]
        [DataRow("begin_goal\n\nend_goal\n")]
        public void Cant_VisitGoalDeclNode_IfNoGoalCountGiven(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);

            // ACT
            ISASNode? res = new SectionVisitor(listener).TryVisitAsGoalState(node.Children[0]);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("begin_goal\n1\nend_goal\n")]
        [DataRow("begin_goal\n0\n0 1\nend_goal\n")]
        [DataRow("begin_goal\n50\n0 1\nend_goal\n")]
        public void Cant_VisitGoalDeclNode_IfNoGoalCountDontMatch(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);

            // ACT
            ISASNode? res = new SectionVisitor(listener).TryVisitAsGoalState(node.Children[0]);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        #endregion

        #region Operator Node

        [TestMethod]
        [DataRow("begin_operator\na\n0\n0\n0\nend_operator\n", "a", 0, 0, 0)]
        [DataRow("begin_operator\na\n1\n0 1\n0\n0\nend_operator\n", "a", 1, 0, 0)]
        [DataRow("begin_operator\na\n0\n0\n1\nend_operator\n", "a", 0, 0, 1)]
        [DataRow("begin_operator\na\n0\n1\n0 0 0 0\n1\nend_operator\n", "a", 0, 1, 1)]
        [DataRow("begin_operator\na\n0\n2\n0 0 0 0\n 1 0 1 6 1 0\n1\nend_operator\n", "a", 0, 2, 1)]
        [DataRow("begin_operator\na\n1\n0 1\n2\n0 0 0 0\n 1 0 1 6 1 0\n1\nend_operator\n", "a", 1, 2, 1)]
        [DataRow("begin_operator\na\n2\n0 1\n2 0\n2\n0 0 0 0\n 1 0 1 6 1 0\n1\nend_operator\n", "a", 2, 2, 1)]
        [DataRow("begin_operator\nab\n2\n0 1\n2 0\n2\n0 0 0 0\n 2 0 1 0 2 6 1 0\n1\nend_operator\n", "ab", 2, 2, 1)]
        public void Can_VisitOperatorNode(string toParse, string expectedName, int expectedPrevails, int expectedEffect, int expectedCost)
        {
            // ARRANGE
            var node = GetParsed(toParse);

            // ACT
            ISASNode? res = new SectionVisitor(null).TryVisitAsOperator(node.Children[0]);

            // ASSERT
            Assert.IsInstanceOfType(res, typeof(OperatorDecl));
            if (res is OperatorDecl ver)
            {
                Assert.AreEqual(expectedName, ver.Name);
                Assert.AreEqual(expectedPrevails, ver.PrevailConditions.Count);
                Assert.AreEqual(expectedEffect, ver.Effects.Count);
                Assert.AreEqual(expectedCost, ver.Cost);
            }
        }

        [TestMethod]
        [DataRow("begin_operator\n0\n0\n0\nend_operator\n")]
        [DataRow("begin_operator\n\n0\n0\n0\nend_operator\n")]
        public void Cant_VisitOperatorNode_IfNoName(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);

            // ACT
            ISASNode? res = new SectionVisitor(listener).TryVisitAsOperator(node.Children[0]);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("begin_operator\na\n\n0\n0\n0\nend_operator\n")]
        [DataRow("begin_operator\na\n\n0\n0\n\n0\nend_operator\n")]
        [DataRow("begin_operator\na\n\n0\n\n0\n\n0\nend_operator\n")]
        public void Cant_VisitOperatorNode_IfContainsEmptyLines(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);

            // ACT
            ISASNode? res = new SectionVisitor(listener).TryVisitAsOperator(node.Children[0]);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("begin_operator\na\n5\n0\n0\nend_operator\n")]
        [DataRow("begin_operator\na\n0\n0 1\n0\n0\nend_operator\n")]
        [DataRow("begin_operator\na\n0\n0 1\n4 1\n0\n0\nend_operator\n")]
        public void Cant_VisitOperatorNode_PrevailCountNotMatch(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);

            // ACT
            ISASNode? res = new SectionVisitor(listener).TryVisitAsOperator(node.Children[0]);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("begin_operator\na\n0\n1\n0\nend_operator\n")]
        [DataRow("begin_operator\na\n0\n0\n0 0 0 0\n0\nend_operator\n")]
        public void Cant_VisitOperatorNode_EffectCountNotMatch(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);

            // ACT
            ISASNode? res = new SectionVisitor(listener).TryVisitAsOperator(node.Children[0]);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("begin_operator\na\n0\n0\nend_operator\n")]
        [DataRow("begin_operator\na\n0\n0\n\nend_operator\n")]
        public void Cant_VisitOperatorNode_IfEffectCostMissing(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);

            // ACT
            ISASNode? res = new SectionVisitor(listener).TryVisitAsOperator(node.Children[0]);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        #endregion

        #region Axiom Node

        [TestMethod]
        [DataRow("begin_rule\n0\n0 0 0\nend_rule\n", 0, 0, 0, 0)]
        [DataRow("begin_rule\n0\n1 0 0\nend_rule\n", 0, 1, 0, 0)]
        [DataRow("begin_rule\n0\n1 0 5\nend_rule\n", 0, 1, 0, 5)]
        [DataRow("begin_rule\n0\n1 10 5\nend_rule\n", 0, 1, 10, 5)]
        [DataRow("begin_rule\n1\n0 1\n1 10 5\nend_rule\n", 1, 1, 10, 5)]
        [DataRow("begin_rule\n2\n0 1\n5 1\n1 10 5\nend_rule\n", 2, 1, 10, 5)]
        public void Can_VisitAxiomNode(string toParse, int expectedConditions, int expectedEffectedVariable, int expectedVariablePrecondition, int expectedNewVariableValue)
        {
            // ARRANGE
            var node = GetParsed(toParse);

            // ACT
            ISASNode? res = new SectionVisitor(null).TryVisitAsAxiom(node.Children[0]);

            // ASSERT
            Assert.IsInstanceOfType(res, typeof(AxiomDecl));
            if (res is AxiomDecl ver)
            {
                Assert.AreEqual(expectedConditions, ver.Conditions.Count);
                Assert.AreEqual(expectedEffectedVariable, ver.EffectedVariable);
                Assert.AreEqual(expectedVariablePrecondition, ver.VariablePrecondition);
                Assert.AreEqual(expectedNewVariableValue, ver.NewVariableValue);
            }
        }

        [TestMethod]
        [DataRow("begin_rule\n1\n0 0 0\nend_rule\n")]
        [DataRow("begin_rule\n10\n0 0 0\nend_rule\n")]
        [DataRow("begin_rule\n0\n0 1\n0 0 0\nend_rule\n")]
        [DataRow("begin_rule\n1\n0 1\n5 0\n0 0 0\nend_rule\n")]
        public void Cant_VisitAxiomNode_IfConditionCountNotMatch(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);

            // ACT
            ISASNode? res = new SectionVisitor(listener).TryVisitAsAxiom(node.Children[0]);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        [TestMethod]
        [DataRow("begin_rule\n0\n0\nend_rule\n")]
        [DataRow("begin_rule\n0\n0 0\nend_rule\n")]
        [DataRow("begin_rule\n0\n0 0 0 0\nend_rule\n")]
        [DataRow("begin_rule\n0\n0 0 0 0 0\nend_rule\n")]
        public void Cant_VisitAxiomNode_IfDenotionNotValid(string toParse)
        {
            // ARRANGE
            var node = GetParsed(toParse);
            IErrorListener listener = new ErrorListener(ParseErrorType.Error);

            // ACT
            ISASNode? res = new SectionVisitor(listener).TryVisitAsAxiom(node.Children[0]);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
        }

        #endregion
    }
}
