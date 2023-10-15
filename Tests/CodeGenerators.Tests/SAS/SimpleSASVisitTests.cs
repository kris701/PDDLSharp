using PDDLSharp;
using PDDLSharp.CodeGenerators;
using PDDLSharp.CodeGenerators.Plans;
using PDDLSharp.CodeGenerators.SAS;
using PDDLSharp.CodeGenerators.Tests;
using PDDLSharp.CodeGenerators.Tests.Plans;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.AST;
using PDDLSharp.Models.Plans;
using PDDLSharp.Models.SAS;
using PDDLSharp.Models.SAS.Sections;
using System;

namespace PDDLSharp.CodeGenerators.Tests.SAS
{
    [TestClass]
    public class SimpleSASVisitTests
    {
        public static IEnumerable<object[]> GetTestData()
        {
            yield return new object[]
            {
                new InitStateDecl(new List<int>(){ 1 }),
                "begin_state\n1\nend_state\n"
            };
            yield return new object[]
            {
                new InitStateDecl(new List<int>(){ 1,2,3 }),
                "begin_state\n1\n2\n3\nend_state\n"
            };

            yield return new object[]
            {
                new GoalStateDecl(new List<ValuePair>()),
                "begin_goal\n0\nend_goal\n"
            };
            yield return new object[]
            {
                new GoalStateDecl(new List<ValuePair>(){ new ValuePair(0, 1) }),
                "begin_goal\n1\n0 1\nend_goal\n"
            };

            yield return new object[]
            {
                new VersionDecl(1),
                "begin_version\n1\nend_version\n"
            };

            yield return new object[]
            {
                new MetricDecl(true),
                "begin_metric\n1\nend_metric\n"
            };
            yield return new object[]
            {
                new MetricDecl(false),
                "begin_metric\n0\nend_metric\n"
            };

        }

        // This is just a test with some random sets of nodes.
        // These tests are not exhaustive!
        [TestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void SmallNodeTests(ISASNode node, string expected)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            ICodeGenerator<ISASNode> generator = new SASCodeGenerator(listener);

            // ACT
            var result = generator.Generate(node);

            // ASSERT
            Assert.AreEqual(expected.Replace("\n", ""), result.Replace(Environment.NewLine, ""));
        }
    }
}