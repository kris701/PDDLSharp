using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.Plans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Grounders.Tests
{
    [TestClass]
    public class ParametizedGrounderTests
    {
        public static IEnumerable<object[]> GetGroundingData_Typeless_NoConstants()
        {
            yield return new object[] {
                new ForAllExp(new ParameterExp(new List<NameExp>(){  }), new AndExp()),
                new List<NameExp>() { new NameExp("obj1"), new NameExp("obj2") },
                1
            };
            yield return new object[] {
                new ForAllExp(new ParameterExp(new List<NameExp>(){ new NameExp("?a") }), new AndExp()),
                new List<NameExp>() { new NameExp("obj1"), new NameExp("obj2") },
                2
            };
            yield return new object[] {
                new ForAllExp(new ParameterExp(new List<NameExp>(){ new NameExp("?a") }), new AndExp()),
                new List<NameExp>() {  },
                0
            };
            yield return new object[] {
                new ForAllExp(new ParameterExp(new List<NameExp>(){ new NameExp("?a"), new NameExp("?b") }), new AndExp()),
                new List<NameExp>() { new NameExp("obj1"), new NameExp("obj2") },
                4
            };

            yield return new object[] {
                new ExistsExp(new ParameterExp(new List<NameExp>(){  }), new AndExp()),
                new List<NameExp>() { new NameExp("obj1"), new NameExp("obj2") },
                1
            };
            yield return new object[] {
                new ExistsExp(new ParameterExp(new List<NameExp>(){ new NameExp("?a") }), new AndExp()),
                new List<NameExp>() { new NameExp("obj1"), new NameExp("obj2") },
                2
            };
            yield return new object[] {
                new ExistsExp(new ParameterExp(new List<NameExp>(){ new NameExp("?a") }), new AndExp()),
                new List<NameExp>() {  },
                0
            };
            yield return new object[] {
                new ExistsExp(new ParameterExp(new List<NameExp>(){ new NameExp("?a"), new NameExp("?b") }), new AndExp()),
                new List<NameExp>() { new NameExp("obj1"), new NameExp("obj2") },
                4
            };
        }

        [TestMethod]
        [DynamicData(nameof(GetGroundingData_Typeless_NoConstants), DynamicDataSourceType.Method)]
        public void Can_GroundCorrectAmount_Typeless_NoConstants(IParametized action, List<NameExp> objs, int expectedCount)
        {
            // ARRANGE
            var decl = new PDDLDecl(new DomainDecl(), new ProblemDecl());
            decl.Problem.Objects = new ObjectsDecl();
            decl.Problem.Objects.Objs = objs;
            IGrounder<IParametized> grounder = new ParametizedGrounder(decl);

            // ACT
            var result = grounder.Ground(action);

            // ASSERT
            Assert.AreEqual(expectedCount, result.Count);
        }

        public static IEnumerable<object[]> GetGroundingData_NoConstants()
        {
            yield return new object[] {
                new ForAllExp(new ParameterExp(new List<NameExp>(){  }), new AndExp()),
                new List<NameExp>() { new NameExp("obj1", new TypeExp("type1")), new NameExp("obj2", new TypeExp("type2")) },
                1
            };
            yield return new object[] {
                new ForAllExp(new ParameterExp(new List<NameExp>(){ new NameExp("?a", new TypeExp("type1")) }), new AndExp()),
                new List<NameExp>() { new NameExp("obj1", new TypeExp("type1")), new NameExp("obj2", new TypeExp("type2")) },
                1
            };
            yield return new object[] {
                new ForAllExp(new ParameterExp(new List<NameExp>(){ new NameExp("?a", new TypeExp("type3")) }), new AndExp()),
                new List<NameExp>() { new NameExp("obj1", new TypeExp("type1")), new NameExp("obj2", new TypeExp("type2")) },
                0
            };
            yield return new object[] {
                new ForAllExp(new ParameterExp(new List<NameExp>(){ new NameExp("?a", new TypeExp("type1")), new NameExp("?b") }), new AndExp()),
                new List<NameExp>() { new NameExp("obj1", new TypeExp("type1")), new NameExp("obj2") },
                2
            };
            yield return new object[] {
                new ForAllExp(new ParameterExp(new List<NameExp>(){ new NameExp("?a", new TypeExp("type1")), new NameExp("?b", new TypeExp("type3")) }), new AndExp()),
                new List<NameExp>() { new NameExp("obj1", new TypeExp("type1")), new NameExp("obj2") },
                0
            };

            yield return new object[] {
                new ExistsExp(new ParameterExp(new List<NameExp>(){  }), new AndExp()),
                new List<NameExp>() { new NameExp("obj1", new TypeExp("type1")), new NameExp("obj2", new TypeExp("type2")) },
                1
            };
            yield return new object[] {
                new ExistsExp(new ParameterExp(new List<NameExp>(){ new NameExp("?a", new TypeExp("type1")) }), new AndExp()),
                new List<NameExp>() { new NameExp("obj1", new TypeExp("type1")), new NameExp("obj2", new TypeExp("type2")) },
                1
            };
            yield return new object[] {
                new ExistsExp(new ParameterExp(new List<NameExp>(){ new NameExp("?a", new TypeExp("type3")) }), new AndExp()),
                new List<NameExp>() { new NameExp("obj1", new TypeExp("type1")), new NameExp("obj2", new TypeExp("type2")) },
                0
            };
            yield return new object[] {
                new ExistsExp(new ParameterExp(new List<NameExp>(){ new NameExp("?a", new TypeExp("type1")), new NameExp("?b") }), new AndExp()),
                new List<NameExp>() { new NameExp("obj1", new TypeExp("type1")), new NameExp("obj2") },
                2
            };
            yield return new object[] {
                new ExistsExp(new ParameterExp(new List<NameExp>(){ new NameExp("?a", new TypeExp("type1")), new NameExp("?b", new TypeExp("type3")) }), new AndExp()),
                new List<NameExp>() { new NameExp("obj1", new TypeExp("type1")), new NameExp("obj2") },
                0
            };
        }

        [TestMethod]
        [DynamicData(nameof(GetGroundingData_NoConstants), DynamicDataSourceType.Method)]
        public void Can_GroundCorrectAmount_NoConstants(IParametized action, List<NameExp> objs, int expectedCount)
        {
            // ARRANGE
            var decl = new PDDLDecl(new DomainDecl(), new ProblemDecl());
            decl.Problem.Objects = new ObjectsDecl();
            decl.Problem.Objects.Objs = objs;
            decl.Domain.Types = new TypesDecl();
            decl.Domain.Types.Add(new TypeExp("type1"));
            decl.Domain.Types.Add(new TypeExp("type2"));
            decl.Domain.Types.Add(new TypeExp("type3"));
            decl.Domain.Types.Add(new TypeExp("type4"));
            IGrounder<IParametized> grounder = new ParametizedGrounder(decl);

            // ACT
            var result = grounder.Ground(action);

            // ASSERT
            Assert.AreEqual(expectedCount, result.Count);
        }
    }
}
