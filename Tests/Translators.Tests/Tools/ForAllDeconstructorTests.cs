using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Translators.Grounders;
using PDDLSharp.Translators.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Translators.Tests.Tools
{
    [TestClass]
    public class ForAllDeconstructorTests
    {
        public static IEnumerable<object[]> DeconstrucForAllData_Valid()
        {
            yield return new object[] {
                new AndExp(new List<IExp>(){
                    new ForAllExp(
                        new ParameterExp(new List<NameExp>(){ new NameExp("?a") }),
                        new PredicateExp("pred", new List<NameExp>(){ new NameExp("?a") }))
                }),
                new AndExp(new List<IExp>(){
                    new AndExp(new List<IExp>()
                    {
                        new PredicateExp("pred", new List<NameExp>(){ new NameExp("obj1") }),
                        new PredicateExp("pred", new List<NameExp>(){ new NameExp("obj2") })
                    })
                }),
                new List<NameExp>(){
                    new NameExp("obj1"),
                    new NameExp("obj2"),
                }
            };

            yield return new object[] {
                new AndExp(new List<IExp>(){
                    new ForAllExp(
                        new ParameterExp(new List<NameExp>(){ new NameExp("?a") }),
                        new PredicateExp("pred", new List<NameExp>(){ new NameExp("?a") }))
                }),
                new AndExp(new List<IExp>(){
                    new AndExp(new List<IExp>()
                    {
                        new PredicateExp("pred", new List<NameExp>(){ new NameExp("obj1") }),
                        new PredicateExp("pred", new List<NameExp>(){ new NameExp("obj2") }),
                        new PredicateExp("pred", new List<NameExp>(){ new NameExp("obj3") }),
                    })
                }),
                new List<NameExp>(){
                    new NameExp("obj1"),
                    new NameExp("obj2"),
                    new NameExp("obj3"),
                }
            };

            yield return new object[] {
                new AndExp(new List<IExp>(){
                    new ForAllExp(
                        new ParameterExp(new List<NameExp>(){ new NameExp("?a"), new NameExp("?b") }),
                        new PredicateExp("pred", new List<NameExp>(){ new NameExp("?a"), new NameExp("?b") }))
                }),
                new AndExp(new List<IExp>(){
                    new AndExp(new List<IExp>()
                    {
                        new PredicateExp("pred", new List<NameExp>(){ new NameExp("obj1"), new NameExp("obj1") }),
                        new PredicateExp("pred", new List<NameExp>(){ new NameExp("obj1"), new NameExp("obj2") }),
                        new PredicateExp("pred", new List<NameExp>(){ new NameExp("obj2"), new NameExp("obj1") }),
                        new PredicateExp("pred", new List<NameExp>(){ new NameExp("obj2"), new NameExp("obj2") }),
                    })
                }),
                new List<NameExp>(){
                    new NameExp("obj1"),
                    new NameExp("obj2"),
                }
            };
        }

        [TestMethod]
        [DynamicData(nameof(DeconstrucForAllData_Valid), DynamicDataSourceType.Method)]
        public void Can_DeconstructForAll(INode input, INode expected, List<NameExp> objects)
        {
            // ARRANGE
            (input as AndExp).Children[0].Parent = input;
            (expected as AndExp).Children[0].Parent = expected;
            var decl = new PDDLDecl(new DomainDecl(), new ProblemDecl());
            decl.Problem.Objects = new ObjectsDecl();
            decl.Problem.Objects.Objs = objects;
            var grounder = new ParametizedGrounder(decl);
            var deconstructor = new ForAllDeconstructor(grounder);

            // ACT
            var result = deconstructor.DeconstructForAlls(input);

            // ASSERT
            Assert.AreEqual(expected, result);
        }
    }
}
