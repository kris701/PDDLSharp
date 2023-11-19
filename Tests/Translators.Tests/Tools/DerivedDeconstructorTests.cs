using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Translators.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Translators.Tests.Tools
{
    [TestClass]
    public class DerivedDeconstructorTests
    {
        public static IEnumerable<object[]> DeconstructDerived_Valid()
        {
            yield return new object[] {
                new AndExp(new List<IExp>()
                {
                    new DerivedPredicateExp("d-pred", new List<DerivedDecl>())
                }),
                new List<DerivedDecl>()
                {
                    new DerivedDecl(new PredicateExp("d-pred"), new PredicateExp("other-check"))
                }
            };
            yield return new object[] {
                new AndExp(new List<IExp>()
                {
                    new DerivedPredicateExp("d-pred", new List<DerivedDecl>()),
                    new DerivedPredicateExp("d-pred2", new List<DerivedDecl>())
                }),
                new List<DerivedDecl>()
                {
                    new DerivedDecl(new PredicateExp("d-pred"), new PredicateExp("other-check")),
                    new DerivedDecl(new PredicateExp("d-pred2"), new PredicateExp("other-check"))
                }
            };
        }

        [TestMethod]
        [DynamicData(nameof(DeconstructDerived_Valid), DynamicDataSourceType.Method)]
        public void Can_DeconstructDerived(IExp expression, List<DerivedDecl> deriveds)
        {
            // ARRANGE
            Assert.IsTrue(expression.FindTypes<DerivedPredicateExp>().Count > 0);
            foreach (var pred in expression.FindTypes<DerivedPredicateExp>())
                foreach(var decl in deriveds)
                    if (decl.Predicate.Name == pred.Name)
                        pred.AddDecl(decl);
            var deconstructor = new DerivedDeconstructor();

            // ACT
            var result = deconstructor.DeconstructDeriveds(expression);

            // ASSERT
            Assert.IsFalse(result.FindTypes<DerivedPredicateExp>().Count > 0);
        }
    }
}
