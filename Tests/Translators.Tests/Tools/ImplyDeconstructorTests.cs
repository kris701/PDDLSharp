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
    public class ImplyDeconstructorTests
    {
        public static IEnumerable<object[]> DeconstrucImplyData_Valid()
        {
            yield return new object[] {
                new AndExp(new List<IExp>(){
                    new ImplyExp(
                        new PredicateExp("ant", new List<NameExp>(){ new NameExp("?a") }),
                        new PredicateExp("con", new List<NameExp>(){ new NameExp("?b") })
                    )
                }),
                new AndExp(new List<IExp>(){
                    new OrExp(new List<IExp>()
                    {
                        new NotExp(new PredicateExp("ant", new List<NameExp>(){ new NameExp("?a") })),
                        new AndExp(new List<IExp>(){
                            new PredicateExp("ant", new List<NameExp>(){ new NameExp("?a") }),
                            new PredicateExp("con", new List<NameExp>(){ new NameExp("?b") })
                        })
                    })
                })
            };

            yield return new object[] {
                new AndExp(new List<IExp>(){
                    new ImplyExp(
                        new PredicateExp("ant", new List<NameExp>(){ new NameExp("?a") }),
                        new PredicateExp("con", new List<NameExp>(){ new NameExp("?b") })
                    ),
                    new ImplyExp(
                        new PredicateExp("ant2", new List<NameExp>(){ new NameExp("?a") }),
                        new PredicateExp("con2", new List<NameExp>(){ new NameExp("?b") })
                    )
                }),
                new AndExp(new List<IExp>(){
                    new OrExp(new List<IExp>()
                    {
                        new NotExp(new PredicateExp("ant", new List<NameExp>(){ new NameExp("?a") })),
                        new AndExp(new List<IExp>(){
                            new PredicateExp("ant", new List<NameExp>(){ new NameExp("?a") }),
                            new PredicateExp("con", new List<NameExp>(){ new NameExp("?b") })
                        })
                    }),
                    new OrExp(new List<IExp>()
                    {
                        new NotExp(new PredicateExp("ant2", new List<NameExp>(){ new NameExp("?a") })),
                        new AndExp(new List<IExp>(){
                            new PredicateExp("ant2", new List<NameExp>(){ new NameExp("?a") }),
                            new PredicateExp("con2", new List<NameExp>(){ new NameExp("?b") })
                        })
                    })
                })
            };
        }

        [TestMethod]
        [DynamicData(nameof(DeconstrucImplyData_Valid), DynamicDataSourceType.Method)]
        public void Can_DeconstructImply(INode input, INode expected)
        {
            // ARRANGE
            (input as AndExp).Children[0].Parent = input;
            (expected as AndExp).Children[0].Parent = expected;
            var deconstructor = new ImplyDeconstructor();

            // ACT
            var result = deconstructor.DeconstructImplies(input);

            // ASSERT
            Assert.AreEqual(expected, result);
        }
    }
}
