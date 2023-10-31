using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.PDDL;
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
    public class ConditionalDeconstructorTests
    {
        public static IEnumerable<object[]> DeconstructWhen_Valid()
        {
            yield return new object[] {
                new ActionDecl(
                    "act",
                    new ParameterExp(),
                    new PredicateExp("a"),
                    new WhenExp(new PredicateExp("b"), new PredicateExp("c"))
                ),
                2
            };
            yield return new object[] {
                new ActionDecl(
                    "act",
                    new ParameterExp(),
                    new PredicateExp("a"),
                    new AndExp(new List<IExp>(){
                        new WhenExp(new PredicateExp("b"), new PredicateExp("c")),
                        new WhenExp(new PredicateExp("d"), new PredicateExp("e"))
                    })
                ),
                6
            };
            yield return new object[] {
                new ActionDecl(
                    "act",
                    new ParameterExp(),
                    new PredicateExp("a"),
                    new AndExp(new List<IExp>(){
                        new WhenExp(new PredicateExp("b"), new PredicateExp("c")),
                        new WhenExp(new PredicateExp("d"), new PredicateExp("e")),
                        new WhenExp(new PredicateExp("f"), new PredicateExp("g"))
                    })
                ),
                14
            };
        }

        [TestMethod]
        [DynamicData(nameof(DeconstructWhen_Valid), DynamicDataSourceType.Method)]
        public void Can_DeconstructWhen(ActionDecl input, int expectedPermutations)
        {
            // ARRANGE
            var deconstructor = new ConditionalDeconstructor();

            // ACT
            var result = deconstructor.DecontructConditionals(input);

            // ASSERT
            Assert.AreEqual(expectedPermutations, result.Count);
        }
    }
}
