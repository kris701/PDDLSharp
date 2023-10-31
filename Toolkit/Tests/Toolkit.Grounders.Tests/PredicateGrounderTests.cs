using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Toolkit.Grounders;

namespace PDDLSharp.Toolkit.Grounders.Tests
{
    [TestClass]
    public class PredicateGrounderTests
    {
        public static IEnumerable<object[]> GetGroundingData_Typeless_NoConstants()
        {
            yield return new object[] {
                new PredicateExp("predicate"),
                new List<NameExp>() { new NameExp("obj1"), new NameExp("obj2") },
                1
            };
            yield return new object[] {
                new PredicateExp("predicate", new List<NameExp>(){ new NameExp("?a") }),
                new List<NameExp>() { new NameExp("obj1"), new NameExp("obj2") },
                2
            };
            yield return new object[] {
                new PredicateExp("predicate", new List<NameExp>(){ new NameExp("?a"),  new NameExp("?b") }),
                new List<NameExp>() { new NameExp("obj1"), new NameExp("obj2") },
                4
            };
            yield return new object[] {
                new PredicateExp("predicate", new List<NameExp>(){ new NameExp("?a"),  new NameExp("?b"),  new NameExp("?c") }),
                new List<NameExp>() { new NameExp("obj1"), new NameExp("obj2") },
                8
            };
            yield return new object[] {
                new PredicateExp("predicate", new List<NameExp>(){ new NameExp("?a"),  new NameExp("?b"),  new NameExp("?c") }),
                new List<NameExp>() { },
                0
            };
            yield return new object[] {
                new PredicateExp("predicate", new List<NameExp>(){ new NameExp("?a"),  new NameExp("?b"),  new NameExp("?c") }),
                new List<NameExp>() { new NameExp("obj1") },
                1
            };
        }

        [TestMethod]
        [DynamicData(nameof(GetGroundingData_Typeless_NoConstants), DynamicDataSourceType.Method)]
        public void Can_GroundCorrectAmount_Typeless_NoConstants(PredicateExp predicate, List<NameExp> objs, int expectedCount)
        {
            // ARRANGE
            var decl = new PDDLDecl(new DomainDecl(), new ProblemDecl());
            decl.Problem.Objects = new ObjectsDecl();
            decl.Problem.Objects.Objs = objs;
            IGrounder<PredicateExp> grounder = new PredicateGrounder(decl);

            // ACT
            var result = grounder.Ground(predicate);

            // ASSERT
            Assert.AreEqual(expectedCount, result.Count);
        }

        public static IEnumerable<object[]> GetGroundingData_NoConstants()
        {
            yield return new object[] {
                new PredicateExp("predicate"),
                new List<NameExp>() { new NameExp("obj1", new TypeExp("type1")), new NameExp("obj2", new TypeExp("type2")) },
                1
            };
            yield return new object[] {
                new PredicateExp("predicate", new List<NameExp>(){ new NameExp("?a", new TypeExp("type1")) }),
                new List<NameExp>() { new NameExp("obj1", new TypeExp("type1")), new NameExp("obj2", new TypeExp("type2")) },
                1
            };
            yield return new object[] {
                new PredicateExp("predicate", new List<NameExp>(){ new NameExp("?a", new TypeExp("type2")) }),
                new List<NameExp>() { new NameExp("obj1", new TypeExp("type1")), new NameExp("obj2", new TypeExp("type2")) },
                1
            };
            yield return new object[] {
                new PredicateExp("predicate", new List<NameExp>(){ new NameExp("?a", new TypeExp("type1")),  new NameExp("?b", new TypeExp("type2")) }),
                new List<NameExp>() { new NameExp("obj1", new TypeExp("type1")), new NameExp("obj2", new TypeExp("type2")) },
                1
            };
            yield return new object[] {
                new PredicateExp("predicate", new List<NameExp>(){ new NameExp("?a", new TypeExp("type1")),  new NameExp("?b", new TypeExp("type2")) }),
                new List<NameExp>() { new NameExp("obj1", new TypeExp("type1")), new NameExp("obj2", new TypeExp("type2")), new NameExp("obj3", new TypeExp("type2")) },
                2
            };
            yield return new object[] {
                new PredicateExp("predicate", new List<NameExp>(){ new NameExp("?a", new TypeExp("type1")),  new NameExp("?b", new TypeExp("type1")),  new NameExp("?c", new TypeExp("type2")) }),
                new List<NameExp>() { new NameExp("obj1", new TypeExp("type1")), new NameExp("obj2", new TypeExp("type1")), new NameExp("obj3", new TypeExp("type2")), new NameExp("obj4", new TypeExp("type2")) },
                8
            };
            yield return new object[] {
                new PredicateExp("predicate", new List<NameExp>(){ new NameExp("?a", new TypeExp("type1")),  new NameExp("?b", new TypeExp("type1")),  new NameExp("?c", new TypeExp("type2")) }),
                new List<NameExp>() { },
                0
            };
        }

        [TestMethod]
        [DynamicData(nameof(GetGroundingData_NoConstants), DynamicDataSourceType.Method)]
        public void Can_GroundCorrectAmount_NoConstants(PredicateExp predicate, List<NameExp> objs, int expectedCount)
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
            IGrounder<PredicateExp> grounder = new PredicateGrounder(decl);

            // ACT
            var result = grounder.Ground(predicate);

            // ASSERT
            Assert.AreEqual(expectedCount, result.Count);
        }

        public static IEnumerable<object[]> GetGroundingData_Typeless()
        {
            yield return new object[] {
                new PredicateExp("predicate"),
                new List<NameExp>() { new NameExp("obj1"), new NameExp("obj2") },
                new List<NameExp>() { new NameExp("con1") },
                1
            };
            yield return new object[] {
                new PredicateExp("predicate", new List<NameExp>(){ new NameExp("?a") }),
                new List<NameExp>() { new NameExp("obj1"), new NameExp("obj2") },
                new List<NameExp>() { new NameExp("con1"), new NameExp("con2") },
                4
            };
            yield return new object[] {
                new PredicateExp("predicate", new List<NameExp>(){ new NameExp("?a"),  new NameExp("?b") }),
                new List<NameExp>() { new NameExp("obj1"), new NameExp("obj2") },
                new List<NameExp>() { new NameExp("con1"), new NameExp("con2") },
                16
            };
        }

        [TestMethod]
        [DynamicData(nameof(GetGroundingData_Typeless), DynamicDataSourceType.Method)]
        public void Can_GroundCorrectAmount_Typeless(PredicateExp predicate, List<NameExp> objs, List<NameExp> constants, int expectedCount)
        {
            // ARRANGE
            var decl = new PDDLDecl(new DomainDecl(), new ProblemDecl());
            decl.Problem.Objects = new ObjectsDecl();
            decl.Problem.Objects.Objs = objs;
            decl.Domain.Constants = new ConstantsDecl();
            decl.Domain.Constants.Constants = constants;
            IGrounder<PredicateExp> grounder = new PredicateGrounder(decl);

            // ACT
            var result = grounder.Ground(predicate);

            // ASSERT
            Assert.AreEqual(expectedCount, result.Count);
        }
    }
}