using PDDLSharp;
using PDDLSharp.ASTGenerators;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.Tests;
using PDDLSharp.Models.Tests.PDDL;
using PDDLSharp.Parsers.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.Tests.PDDL
{
    [TestClass]
    public class EqualityTests
    {
        public static IEnumerable<object[]> GetTestData()
        {
            // Name Exp Node
            yield return new object[]
            {
                new NameExp(new ASTNode(), null, "abc"),
                new NameExp(new ASTNode(), null, "abc"),
                true
            };
            yield return new object[]
            {
                new NameExp(new ASTNode(1, 10, "(abc)", "abc"), null, "abc"),
                new NameExp(new ASTNode(1, 10, "(abc)", "abc"), null, "abc"),
                true
            };
            yield return new object[]
            {
                new NameExp(new ASTNode(), null, "abc", new TypeExp(new ASTNode(), null, "type")),
                new NameExp(new ASTNode(), null, "abc", new TypeExp(new ASTNode(), null, "type")),
                true
            };

            // Predicate Exp Node
            yield return new object[]
            {
                new PredicateExp("abc"),
                new PredicateExp("abc"),
                true
            };
            yield return new object[]
            {
                new PredicateExp("abc", new List<NameExp>(){ new NameExp("?a") }),
                new PredicateExp("abc"),
                false
            };
            yield return new object[]
            {
                new PredicateExp("abc", new List<NameExp>(){ new NameExp("?a") }),
                new PredicateExp("abc", new List<NameExp>(){ new NameExp("?a") }),
                true
            };
            yield return new object[]
            {
                new PredicateExp("abc", new List<NameExp>(){ new NameExp("?a"), new NameExp("?b") }),
                new PredicateExp("abc", new List<NameExp>(){ new NameExp("?a"), new NameExp("?b")  }),
                true
            };
            yield return new object[]
            {
                new PredicateExp("abc", new List<NameExp>(){ new NameExp("?b"), new NameExp("?a") }),
                new PredicateExp("abc", new List<NameExp>(){ new NameExp("?a"), new NameExp("?b")  }),
                false
            };

            // And Exp Node
            yield return new object[]
            {
                new AndExp(new ASTNode(), null, new List<IExp>()
                {
                    new NameExp(new ASTNode(), null, "abc"),
                    new NameExp(new ASTNode(), null, "def")
                }),
                new AndExp(new ASTNode(), null, new List<IExp>()
                {
                    new NameExp(new ASTNode(), null, "abc"),
                    new NameExp(new ASTNode(), null, "def")
                }),
                true
            };
            yield return new object[]
            {
                new AndExp(new ASTNode(), null, new List<IExp>()
                {
                    new NameExp(new ASTNode(), null, "def"),
                    new NameExp(new ASTNode(), null, "abc")
                }),
                new AndExp(new ASTNode(), null, new List<IExp>()
                {
                    new NameExp(new ASTNode(), null, "abc"),
                    new NameExp(new ASTNode(), null, "def")
                }),
                true
            };
            yield return new object[]
            {
                new AndExp(new ASTNode(), null, new List<IExp>()
                {
                    new NameExp(new ASTNode(), null, "def"),
                    new NameExp(new ASTNode(), null, "abc")
                }),
                new AndExp(new ASTNode(), null, new List<IExp>()
                {
                    new NameExp(new ASTNode(), null, "def")
                }),
                false
            };

            // Extends Decl
            yield return new object[]
            {
                new ExtendsDecl(new ASTNode(), null, new List<NameExp>()
                {
                    new NameExp(new ASTNode(), null, "abc"),
                    new NameExp(new ASTNode(), null, "def")
                }),
                new ExtendsDecl(new ASTNode(), null, new List<NameExp>()
                {
                    new NameExp(new ASTNode(), null, "abc"),
                    new NameExp(new ASTNode(), null, "def")
                }),
                true
            };

            // Action Decl
            yield return new object[]
            {
                new ActionDecl(
                    "act1",
                    new ParameterExp(new List<NameExp>(){

                    }),
                    new AndExp(new List<IExp>()
                    {

                    }),new AndExp(new List<IExp>()
                    {

                    })),
                new ActionDecl(
                    "act1",
                    new ParameterExp(new List<NameExp>(){

                    }),
                    new AndExp(new List<IExp>()
                    {

                    }),new AndExp(new List<IExp>()
                    {

                    })),
                true
            };
            yield return new object[]
            {
                new ActionDecl(
                    "act1",
                    new ParameterExp(new List<NameExp>(){
                        new NameExp("?a")
                    }),
                    new AndExp(new List<IExp>()
                    {

                    }),new AndExp(new List<IExp>()
                    {

                    })),
                new ActionDecl(
                    "act1",
                    new ParameterExp(new List<NameExp>(){
                        new NameExp("?a")
                    }),
                    new AndExp(new List<IExp>()
                    {

                    }),new AndExp(new List<IExp>()
                    {

                    })),
                true
            };
            yield return new object[]
            {
                new ActionDecl(
                    "act1",
                    new ParameterExp(new List<NameExp>(){
                        new NameExp("?a"),
                        new NameExp("?b")
                    }),
                    new AndExp(new List<IExp>()
                    {

                    }),new AndExp(new List<IExp>()
                    {

                    })),
                new ActionDecl(
                    "act1",
                    new ParameterExp(new List<NameExp>(){
                        new NameExp("?b"),
                        new NameExp("?a")
                    }),
                    new AndExp(new List<IExp>()
                    {

                    }),new AndExp(new List<IExp>()
                    {

                    })),
                true
            };
            yield return new object[]
            {
                new ActionDecl(
                    "act1",
                    new ParameterExp(new List<NameExp>(){
                        new NameExp("?a"),
                        new NameExp("?b")
                    }),
                    new AndExp(new List<IExp>()
                    {
                        new PredicateExp("pred")
                    }),new AndExp(new List<IExp>()
                    {

                    })),
                new ActionDecl(
                    "act1",
                    new ParameterExp(new List<NameExp>(){
                        new NameExp("?a"),
                        new NameExp("?b")
                    }),
                    new AndExp(new List<IExp>()
                    {

                    }),new AndExp(new List<IExp>()
                    {

                    })),
                false
            };
            yield return new object[]
            {
                new ActionDecl(
                    "act1",
                    new ParameterExp(new List<NameExp>(){
                        new NameExp("?a"),
                        new NameExp("?b")
                    }),
                    new AndExp(new List<IExp>()
                    {
                        new PredicateExp("pred")
                    }),new AndExp(new List<IExp>()
                    {
                        new PredicateExp("pred"),
                        new NotExp(new PredicateExp("pred"))
                    })),
                new ActionDecl(
                    "act1",
                    new ParameterExp(new List<NameExp>(){
                        new NameExp("?a"),
                        new NameExp("?b")
                    }),
                    new AndExp(new List<IExp>()
                    {
                        new PredicateExp("pred")
                    }),new AndExp(new List<IExp>()
                    {
                        new PredicateExp("pred"),
                        new NotExp(new PredicateExp("pred"))
                    })),
                true
            };
            yield return new object[]
            {
                new ActionDecl(
                    "act1",
                    new ParameterExp(new List<NameExp>(){
                        new NameExp("?a"),
                        new NameExp("?b")
                    }),
                    new AndExp(new List<IExp>()
                    {
                        new PredicateExp("pred")
                    }),new AndExp(new List<IExp>()
                    {
                        new PredicateExp("pred"),
                        new NotExp(new PredicateExp("pred"))
                    })),
                new ActionDecl(
                    "act1",
                    new ParameterExp(new List<NameExp>(){
                        new NameExp("?a"),
                        new NameExp("?b")
                    }),
                    new AndExp(new List<IExp>()
                    {
                        new PredicateExp("pred")
                    }),new AndExp(new List<IExp>()
                    {
                        new PredicateExp("pred"),
                        new NotExp(new PredicateExp("pred"))
                    })),
                true
            };
        }

        // This is just a test with some random sets of nodes.
        // These tests are not exhaustive!
        [TestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void Can_Equals(INode first, INode second, bool expected)
        {
            // ARRANGE
            // ACT
            var result = first.Equals(second);

            // ASSERT
            Assert.IsTrue(result == expected);
        }
    }
}
