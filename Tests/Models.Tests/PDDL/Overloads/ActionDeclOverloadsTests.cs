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
    public class ActionDeclOverloadsTests
    {
        [TestMethod]
        public void Can_AddAndIfMissing()
        {
            // ARRANGE
            var action = new ActionDecl("action");
            action.Preconditions = new PredicateExp("test1");
            action.Effects = new PredicateExp("test1");
            Assert.IsFalse(action.Preconditions is AndExp);
            Assert.IsFalse(action.Effects is AndExp);

            // ACT
            action.EnsureAnd();

            // ASSERT
            Assert.IsTrue(action.Preconditions is AndExp);
            Assert.IsTrue(action.Effects is AndExp);
        }

        [TestMethod]
        public void Can_CanAnnonimise()
        {
            // ARRANGE
            var action = new ActionDecl("action");
            action.Parameters = new ParameterExp(new List<NameExp>() { new NameExp("?arg1"), new NameExp("?arg2") });
            action.Preconditions = new PredicateExp("pred", new List<NameExp>() { new NameExp("?arg1") });
            action.Effects = new PredicateExp("pred", new List<NameExp>() { new NameExp("?arg2") });
            Assert.AreEqual("action", action.Name);
            Assert.AreEqual("?arg1", action.Parameters.Values[0].Name);
            Assert.AreEqual("?arg2", action.Parameters.Values[1].Name);
            Assert.AreEqual(2, action.FindNames("?arg1").Count);
            Assert.AreEqual(2, action.FindNames("?arg2").Count);

            // ACT
            action = action.Annonymise();

            // ASSERT
            Assert.AreEqual("Name", action.Name);
            Assert.AreEqual("?0", action.Parameters.Values[0].Name);
            Assert.AreEqual("?1", action.Parameters.Values[1].Name);
            Assert.AreEqual(0, action.FindNames("?arg1").Count);
            Assert.AreEqual(0, action.FindNames("?arg2").Count);
        }

        [TestMethod]
        public void Can_GetDistinct_1()
        {
            // ARRANGE
            var action1 = new ActionDecl("action-name");
            action1.Preconditions = new PredicateExp("abc");
            var action2 = new ActionDecl("action-name");
            action2.Preconditions = new PredicateExp("abc");
            var action3 = new ActionDecl("action-name");
            action3.Preconditions = new PredicateExp("abc");
            var actions = new List<ActionDecl>()
            {
                action1,
                action2,
                action3
            };

            // ACT
            var distinc = actions.Distinct();

            // ASSERT
            Assert.AreEqual(1, distinc.Count);
        }

        [TestMethod]
        public void Can_GetDistinct_2()
        {
            // ARRANGE
            var action1 = new ActionDecl("action-name");
            action1.Preconditions = new PredicateExp("abc");
            var action2 = new ActionDecl("action-name");
            action2.Preconditions = new PredicateExp("abc");
            var action3 = new ActionDecl("action-name_2");
            action3.Effects = new PredicateExp("abc");
            var actions = new List<ActionDecl>()
            {
                action1,
                action2,
                action3
            };

            // ACT
            var distinc = actions.Distinct();

            // ASSERT
            Assert.AreEqual(2, distinc.Count);
        }

        [TestMethod]
        public void Can_GetDistinct_3()
        {
            // ARRANGE
            var action1 = new ActionDecl("action-name");
            var action2 = new ActionDecl("action-name");
            var action3 = new ActionDecl("action-name");
            var actions = new List<ActionDecl>()
            {
                action1,
                action2,
                action3
            };

            // ACT
            var distinc = actions.Distinct(actions);

            // ASSERT
            Assert.AreEqual(0, distinc.Count);
        }
    }
}
