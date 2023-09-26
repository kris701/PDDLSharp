using PDDL.ErrorListeners;
using PDDL.Models;
using PDDL.Models.AST;
using PDDL.Models.Expressions;
using System;

namespace PDDL.CodeGenerators.Tests
{
    [TestClass]
    public class SimpleVisitTests
    {
        public static IEnumerable<object[]> GetTestData()
        {
            // Name Exp Node
            yield return new object[]
            {
                new NameExp(new ASTNode(), null, "abc"),
                "(abc)"
            };
            yield return new object[]
            {
                new NameExp(new ASTNode(), null, "abc", new TypeExp(new ASTNode(), null, "typeName")),
                "(abc - typeName)"
            };

            // And Exp Node
            yield return new object[]
            {
                new AndExp(new ASTNode(), null, new List<IExp>()
                {
                    new NameExp(new ASTNode(), null, "abc", new TypeExp(new ASTNode(), null, "typeName")),
                    new NameExp(new ASTNode(), null, "abc"),
                }),
                "(and (abc - typeName) (abc))"
            };
            yield return new object[]
            {
                new AndExp(new ASTNode(), null, new List<IExp>()
                {
                    new NameExp(new ASTNode(), null, "abc", new TypeExp(new ASTNode(), null, "typeName")),
                }),
                "(and (abc - typeName))"
            };
        }

        // This is just a test with some random sets of nodes.
        // These tests are not exhaustive!
        [TestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void SmallNodeTests(INode node, string expected)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IPDDLCodeGenerator generator = new PDDLCodeGenerator(listener);

            // ACT
            var result = generator.Generate(node);

            // ASSERT
            Assert.AreEqual(expected.Replace(Environment.NewLine, ""), result.Replace(Environment.NewLine, ""));
        }
    }
}