using PDDL.ASTGenerator;
using PDDL.ErrorListeners;
using PDDL.Models.AST;
using PDDL.Models.Domain;
using PDDL.Parsers.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDL.Models.Tests
{
    [TestClass]
    public class EqualityTests
    {
        public static IEnumerable<object[]> GetSameTestData()
        {
            yield return new object[]
            {
                new NameExp(new ASTNode(), null, "abc"),
                new NameExp(new ASTNode(), null, "abc")
            };
        }

        [TestMethod]
        [DynamicData(nameof(GetSameTestData), DynamicDataSourceType.Method)]
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
