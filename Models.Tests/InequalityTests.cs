﻿using PDDLSharp.ASTGenerators;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.AST;
using PDDLSharp.Models.Domain;
using PDDLSharp.Models.Expressions;
using PDDLSharp.Parsers.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.Tests
{
    [TestClass]
    public class InequalityTests
    {
        public static IEnumerable<object[]> GetTestData()
        {
            // Name Exp Node
            yield return new object[]
            {
                new NameExp(new ASTNode(), null, "abc"),
                new NameExp(new ASTNode(), null, "abcc")
            };
            yield return new object[]
            {
                new NameExp(new ASTNode(), null, "abc", new TypeExp(new ASTNode(), null, "type")),
                new NameExp(new ASTNode(), null, "abc", new TypeExp(new ASTNode(), null, "type 2"))
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
                    new NameExp(new ASTNode(), null, "qqq"),
                    new NameExp(new ASTNode(), null, "a111")
                })
            };

            // Extends Decl
            yield return new object[]
            {
                new ExtendsDecl(new ASTNode(), null, new List<NameExp>()
                {
                    new NameExp(new ASTNode(), null, " "),
                    new NameExp(new ASTNode(), null, "def")
                }),
                new ExtendsDecl(new ASTNode(), null, new List<NameExp>()
                {
                    new NameExp(new ASTNode(), null, "abc"),
                    new NameExp(new ASTNode(), null, "def")
                })
            };
        }

        // This is just a test with some random sets of nodes.
        // These tests are not exhaustive!
        [TestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void Equal_False_If_Not_Equal(INode first, INode second)
        {
            // ARRANGE
            // ACT
            var result = first.Equals(second);

            // ASSERT
            Assert.IsFalse(result);
        }
    }
}