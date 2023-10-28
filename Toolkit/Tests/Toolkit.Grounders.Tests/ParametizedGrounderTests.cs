using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.Plans;
using PDDLSharp.Parsers.PDDL;
using PDDLSharp.Parsers;
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
        #region ForAll and Exists
        public static IEnumerable<object[]> GetGroundingData_Typeless_NoConstants_ForAllAndExists()
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
        [DynamicData(nameof(GetGroundingData_Typeless_NoConstants_ForAllAndExists), DynamicDataSourceType.Method)]
        public void Can_GroundCorrectAmount_Typeless_NoConstants_ForAllAndExists(IParametized action, List<NameExp> objs, int expectedCount)
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

        public static IEnumerable<object[]> GetGroundingData_NoConstants_ForAllAndExists()
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
        [DynamicData(nameof(GetGroundingData_NoConstants_ForAllAndExists), DynamicDataSourceType.Method)]
        public void Can_GroundCorrectAmount_NoConstants_ForAllAndExists(IParametized action, List<NameExp> objs, int expectedCount)
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

        #endregion

        #region ActionDecl

        [TestMethod]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob01/prob01.pddl",
    // Move
    "TestData/gripper/prob01/expected1.pddl",
    "TestData/gripper/prob01/expected2.pddl",
    "TestData/gripper/prob01/expected3.pddl",
    "TestData/gripper/prob01/expected4.pddl",

    // Pick
    "TestData/gripper/prob01/expected5.pddl",
    "TestData/gripper/prob01/expected6.pddl",
    "TestData/gripper/prob01/expected7.pddl",
    "TestData/gripper/prob01/expected8.pddl",

    "TestData/gripper/prob01/expected9.pddl",
    "TestData/gripper/prob01/expected10.pddl",
    "TestData/gripper/prob01/expected11.pddl",
    "TestData/gripper/prob01/expected12.pddl",

    // Drop
    "TestData/gripper/prob01/expected13.pddl",
    "TestData/gripper/prob01/expected14.pddl",
    "TestData/gripper/prob01/expected15.pddl",
    "TestData/gripper/prob01/expected16.pddl",

    "TestData/gripper/prob01/expected17.pddl",
    "TestData/gripper/prob01/expected18.pddl",
    "TestData/gripper/prob01/expected19.pddl",
    "TestData/gripper/prob01/expected20.pddl"
)]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob01small/prob01small.pddl",
    // Move
    "TestData/gripper/prob01/expected1.pddl",
    "TestData/gripper/prob01/expected2.pddl",
    "TestData/gripper/prob01/expected3.pddl",
    "TestData/gripper/prob01/expected4.pddl"
)]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob01zero/prob01zero.pddl")]
        [DataRow("TestData/logistics98/domain.pddl", "TestData/logistics98/prob01zero/prob01zero.pddl")]
        [DataRow("TestData/logistics98/domain.pddl", "TestData/logistics98/prob01small/prob01small.pddl",
    // LOAD-TRUCK
    "TestData/logistics98/prob01small/expected1.pddl",

    // UNLOAD-TRUCK
    "TestData/logistics98/prob01small/expected2.pddl"
)]
        public void Can_FullGroundDomains(string domain, string problem, params string[] expecteds)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<INode> parser = new PDDLParser(listener);
            var decl = new PDDLDecl(
                parser.ParseAs<DomainDecl>(new FileInfo(domain)),
                parser.ParseAs<ProblemDecl>(new FileInfo(problem)));
            IGrounder<IParametized> grounder = new ParametizedGrounder(decl);

            // ACT
            var results = new List<ActionDecl>();
            foreach (var act in decl.Domain.Actions)
                results.AddRange(grounder.Ground(act).Cast<ActionDecl>());

            // ASSERT
            Assert.AreEqual(expecteds.Length, results.Count);
            foreach (var expected in expecteds)
            {
                var expectedAct = parser.ParseAs<ActionDecl>(new FileInfo(expected));
                Assert.IsTrue(results.Contains(expectedAct));
            }
        }

        public static IEnumerable<object[]> GetGroundingData_Typeless_NoConstants_Actions()
        {
            yield return new object[] {
                new ActionDecl("act", new ParameterExp(new List<NameExp>(){  }), new AndExp(), new AndExp()),
                new List<NameExp>() { new NameExp("obj1"), new NameExp("obj2") },
                1
            };
            yield return new object[] {
                new ActionDecl("act", new ParameterExp(new List<NameExp>(){ new NameExp("?a") }), new AndExp(), new AndExp()),
                new List<NameExp>() { new NameExp("obj1"), new NameExp("obj2") },
                2
            };
            yield return new object[] {
                new ActionDecl("act", new ParameterExp(new List<NameExp>(){ new NameExp("?a") }), new AndExp(), new AndExp()),
                new List<NameExp>() {  },
                0
            };
            yield return new object[] {
                new ActionDecl("act", new ParameterExp(new List<NameExp>(){ new NameExp("?a"), new NameExp("?b") }), new AndExp(), new AndExp()),
                new List<NameExp>() { new NameExp("obj1"), new NameExp("obj2") },
                4
            };
        }

        [TestMethod]
        [DynamicData(nameof(GetGroundingData_Typeless_NoConstants_Actions), DynamicDataSourceType.Method)]
        public void Can_GroundCorrectAmount_Typeless_NoConstants_Actions(ActionDecl action, List<NameExp> objs, int expectedCount)
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

        public static IEnumerable<object[]> GetGroundingData_NoConstants_Actions()
        {
            yield return new object[] {
                new ActionDecl("act", new ParameterExp(new List<NameExp>(){  }), new AndExp(), new AndExp()),
                new List<NameExp>() { new NameExp("obj1", new TypeExp("type1")), new NameExp("obj2", new TypeExp("type2")) },
                1
            };
            yield return new object[] {
                new ActionDecl("act", new ParameterExp(new List<NameExp>(){ new NameExp("?a", new TypeExp("type1")) }), new AndExp(), new AndExp()),
                new List<NameExp>() { new NameExp("obj1", new TypeExp("type1")), new NameExp("obj2", new TypeExp("type2")) },
                1
            };
            yield return new object[] {
                new ActionDecl("act", new ParameterExp(new List<NameExp>(){ new NameExp("?a", new TypeExp("type3")) }), new AndExp(), new AndExp()),
                new List<NameExp>() { new NameExp("obj1", new TypeExp("type1")), new NameExp("obj2", new TypeExp("type2")) },
                0
            };
            yield return new object[] {
                new ActionDecl("act", new ParameterExp(new List<NameExp>(){ new NameExp("?a", new TypeExp("type1")), new NameExp("?b") }), new AndExp(), new AndExp()),
                new List<NameExp>() { new NameExp("obj1", new TypeExp("type1")), new NameExp("obj2") },
                2
            };
            yield return new object[] {
                new ActionDecl("act", new ParameterExp(new List<NameExp>(){ new NameExp("?a", new TypeExp("type1")), new NameExp("?b", new TypeExp("type3")) }), new AndExp(), new AndExp()),
                new List<NameExp>() { new NameExp("obj1", new TypeExp("type1")), new NameExp("obj2") },
                0
            };
        }

        [TestMethod]
        [DynamicData(nameof(GetGroundingData_NoConstants_Actions), DynamicDataSourceType.Method)]
        public void Can_GroundCorrectAmount_NoConstants_Actions(ActionDecl action, List<NameExp> objs, int expectedCount)
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

        #endregion
    }
}
