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
    public class PredicateExpOverloadsTests
    {
        [TestMethod]
        public void Can_DetectTrueOnlyPredicate_1()
        {
            // ARRANGE
            var predicate = new PredicateExp("some-pred");
            var notPredicate = new NotExp(predicate);
            var domain = new DomainDecl();
            domain.Actions.Add(new ActionDecl("action")
            {
                Effects = predicate
            });

            // ACT
            var result = predicate.CanOnlyBeSetToTrue(domain);

            // ASSERT
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Can_DetectTrueOnlyPredicate_2()
        {
            // ARRANGE
            var predicate = new PredicateExp("some-pred");
            var notPredicate = new NotExp(predicate);
            predicate.Parent = notPredicate;
            var domain = new DomainDecl();
            domain.Actions.Add(new ActionDecl("action")
            {
                Effects = notPredicate
            });

            // ACT
            var result = predicate.CanOnlyBeSetToTrue(domain);

            // ASSERT
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Can_DetectFalseOnlyPredicate_1()
        {
            // ARRANGE
            var predicate = new PredicateExp("some-pred");
            var notPredicate = new NotExp(predicate);
            var domain = new DomainDecl();
            domain.Actions.Add(new ActionDecl("action")
            {
                Effects = predicate
            });

            // ACT
            var result = predicate.CanOnlyBeSetToFalse(domain);

            // ASSERT
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Can_DetectFalseOnlyPredicate_2()
        {
            // ARRANGE
            var predicate = new PredicateExp("some-pred");
            var notPredicate = new NotExp(predicate);
            predicate.Parent = notPredicate;
            var domain = new DomainDecl();
            domain.Actions.Add(new ActionDecl("action")
            {
                Effects = notPredicate
            });

            // ACT
            var result = predicate.CanOnlyBeSetToFalse(domain);

            // ASSERT
            Assert.IsTrue(result);
        }
    }
}
