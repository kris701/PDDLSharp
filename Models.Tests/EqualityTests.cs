using PDDLSharp.ASTGenerators;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Parsers.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.Tests
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
                new NameExp(new ASTNode(), null, "abc")
            };
            yield return new object[]
            {
                new NameExp(new ASTNode(1, 10, "(abc)", "abc"), null, "abc"),
                new NameExp(new ASTNode(1, 10, "(abc)", "abc"), null, "abc")
            };
            yield return new object[]
            {
                new NameExp(new ASTNode(), null, "abc", new TypeExp(new ASTNode(), null, "type")),
                new NameExp(new ASTNode(), null, "abc", new TypeExp(new ASTNode(), null, "type"))
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
                })
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
                })
            };
        }

        // This is just a test with some random sets of nodes.
        // These tests are not exhaustive!
        [TestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void Equal_True_If_Equal(INode first, INode second)
        {
            // ARRANGE
            // ACT
            var result = first.Equals(second);

            // ASSERT
            Assert.IsTrue(result);
        }
    }
}
