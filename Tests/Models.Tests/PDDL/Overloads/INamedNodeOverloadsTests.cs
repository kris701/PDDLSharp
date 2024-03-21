using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Overloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.Tests.PDDL.Overloads
{
    [TestClass]
    public class INamedNodeOverloadsTests
    {
        public static IEnumerable<object[]> GetTestData()
        {
            yield return new object[]
            {
                new PredicateExp("pred")
            };
            yield return new object[]
            {
                new ActionDecl("action")
            };
            yield return new object[]
            {
                new NameExp("?arg1")
            };
        }

        [TestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void Can_Annonymise(INamedNode node)
        {
            Assert.AreNotEqual("Name", node.Name);
            var result = node.Annonymise();
            Assert.AreEqual("Name", result.Name);
        }

        [TestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void Can_Prefix(INamedNode node)
        {
            var prefix = "prefix-text-";
            Assert.AreNotEqual($"{prefix}{node.Name}", node.Name);
            var result = node.Prefix(prefix);
            Assert.AreEqual($"{prefix}{node.Name}", result.Name);
        }

        [TestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void Can_Sufix(INamedNode node)
        {
            var sufix = "-some-sufix";
            Assert.AreNotEqual($"{node.Name}{sufix}", node.Name);
            var result = node.Sufix(sufix);
            Assert.AreEqual($"{node.Name}{sufix}", result.Name);
        }
    }
}
