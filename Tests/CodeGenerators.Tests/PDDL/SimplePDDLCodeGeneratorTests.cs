using PDDLSharp;
using PDDLSharp.CodeGenerators;
using PDDLSharp.CodeGenerators.PDDL;
using PDDLSharp.CodeGenerators.Tests;
using PDDLSharp.CodeGenerators.Tests.PDDL;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using System;

namespace PDDLSharp.CodeGenerators.Tests.PDDL
{
    [TestClass]
    public class SimplePDDLCodeGeneratorTests
    {
        public static IEnumerable<object[]> GetTestData()
        {
            // Name Exp Node
            yield return new object[]
            {
                new NameExp(null, "abc"),
                "(abc)"
            };
            yield return new object[]
            {
                new NameExp(null, "abc", new TypeExp("type")),
                "(abc)"
            };

            // Predicates Decl Node
            yield return new object[]
            {
                new PredicatesDecl(null, new List<PredicateExp>()
                {
                    new PredicateExp(null, "name", new List<NameExp>()
                    {
                        new NameExp(null, "?a"),
                        new NameExp(null, "?b")
                    })
                }),
                "(:predicates(name ?a - object ?b - object))"
            };
            yield return new object[]
            {
                new PredicatesDecl(null, new List<PredicateExp>()
                {
                    new PredicateExp(null, "name", new List<NameExp>()
                    {
                        new NameExp(null, "?a", new TypeExp("typea")),
                        new NameExp(null, "?b")
                    })
                }),
                "(:predicates(name ?a - typea ?b - object))"
            };
            yield return new object[]
            {
                new PredicatesDecl(null, new List<PredicateExp>()
                {
                    new PredicateExp(null, "name", new List<NameExp>()
                    {
                        new NameExp(null, "?a", new TypeExp("typea")),
                        new NameExp(null, "?b", new TypeExp("typeb"))
                    })
                }),
                "(:predicates(name ?a - typea ?b - typeb))"
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
            ICodeGenerator<INode> generator = new PDDLCodeGenerator(listener);

            // ACT
            var result = generator.Generate(node);

            // ASSERT
            Assert.AreEqual(expected.Replace(Environment.NewLine, ""), result.Replace(Environment.NewLine, ""));
        }
    }
}