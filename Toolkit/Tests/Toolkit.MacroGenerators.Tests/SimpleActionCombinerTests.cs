using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.MacroGenerators.Tests
{
    [TestClass]
    public class SimpleActionCombinerTests
    {
        public static IEnumerable<object[]> GetCombineEffPreData()
        {
            yield return new object[] {
                new List<ActionDecl>()
                {
                    new ActionDecl("act1",
                        new ParameterExp(new List<NameExp>(){ new NameExp("?a") }),
                        new AndExp(new List<IExp>()
                        {
                            new PredicateExp("pred1")
                        }),
                        new AndExp(new List<IExp>()
                        {
                            new NotExp(new PredicateExp("pred1"))
                        })),
                },
                1,
                1
            };
            yield return new object[] {
                new List<ActionDecl>()
                {
                    new ActionDecl("act1",
                        new ParameterExp(new List<NameExp>(){ new NameExp("?a") }),
                        new AndExp(new List<IExp>()
                        {
                            new PredicateExp("pred1")
                        }),
                        new AndExp(new List<IExp>()
                        {
                            new NotExp(new PredicateExp("pred1"))
                        })),
                    new ActionDecl("act2",
                        new ParameterExp(new List<NameExp>(){ new NameExp("?a") }),
                        new AndExp(new List<IExp>()
                        {
                            new PredicateExp("pred1")
                        }),
                        new AndExp(new List<IExp>()
                        {
                            new NotExp(new PredicateExp("pred1"))
                        })),
                },
                1,
                1
            };
            yield return new object[] {
                new List<ActionDecl>()
                {
                    new ActionDecl("act1",
                        new ParameterExp(new List<NameExp>(){ new NameExp("?a") }),
                        new AndExp(new List<IExp>()
                        {
                            new PredicateExp("pred1")
                        }),
                        new AndExp(new List<IExp>()
                        {
                            new NotExp(new PredicateExp("pred1"))
                        })),
                    new ActionDecl("act2",
                        new ParameterExp(new List<NameExp>(){ new NameExp("?a") }),
                        new AndExp(new List<IExp>()
                        {
                            new PredicateExp("pred2")
                        }),
                        new AndExp(new List<IExp>()
                        {
                            new NotExp(new PredicateExp("pred1"))
                        })),
                },
                2,
                1
            };
            yield return new object[] {
                new List<ActionDecl>()
                {
                    new ActionDecl("act1",
                        new ParameterExp(new List<NameExp>(){ new NameExp("?a") }),
                        new AndExp(new List<IExp>()
                        {
                            new PredicateExp("pred1"),
                            new PredicateExp("pred2"),
                        }),
                        new AndExp(new List<IExp>()
                        {
                            new NotExp(new PredicateExp("pred1"))
                        })),
                    new ActionDecl("act2",
                        new ParameterExp(new List<NameExp>(){ new NameExp("?a") }),
                        new AndExp(new List<IExp>()
                        {
                            new PredicateExp("pred2"),
                            new NotExp(new PredicateExp("pred1")),
                            new NotExp(new PredicateExp("pred3"))
                        }),
                        new AndExp(new List<IExp>()
                        {
                            new NotExp(new PredicateExp("pred2"))
                        })),
                },
                3,
                2
            };
        }

        [TestMethod]
        [DynamicData(nameof(GetCombineEffPreData), DynamicDataSourceType.Method)]
        public void Can_Combine_EffectsAndPreconditionCount(List<ActionDecl> actions, int expectedPreCount, int expectedEffCount)
        {
            // ARRANGE
            var combiner = new SimpleActionCombiner();

            // ACT
            var result = combiner.Combine(actions);

            // ASSERT
            Assert.AreEqual(expectedPreCount, (result.Preconditions as AndExp).Children.Count);
            Assert.AreEqual(expectedEffCount, (result.Effects as AndExp).Children.Count);
        }

        public static IEnumerable<object[]> GetCombineParameters()
        {
            yield return new object[] {
                new List<ActionDecl>()
                {
                    new ActionDecl("act1",
                        new ParameterExp(new List<NameExp>(){ new NameExp("?a") }),
                        new AndExp(new List<IExp>()
                        {
                            new PredicateExp("pred1")
                        }),
                        new AndExp(new List<IExp>()
                        {
                            new NotExp(new PredicateExp("pred1"))
                        })),
                },
                0
            };
            yield return new object[] {
                new List<ActionDecl>()
                {
                    new ActionDecl("act1",
                        new ParameterExp(new List<NameExp>(){ new NameExp("?a") }),
                        new AndExp(new List<IExp>()
                        {
                            new PredicateExp("pred1", new List<NameExp>(){ new NameExp("?a") })
                        }),
                        new AndExp(new List<IExp>()
                        {
                            new NotExp(new PredicateExp("pred1", new List<NameExp>(){ new NameExp("?a") }))
                        })),
                },
                1
            };
            yield return new object[] {
                new List<ActionDecl>()
                {
                    new ActionDecl("act1",
                        new ParameterExp(new List<NameExp>(){ new NameExp("?a") }),
                        new AndExp(new List<IExp>()
                        {
                            new PredicateExp("pred1", new List<NameExp>(){ new NameExp("?a") })
                        }),
                        new AndExp(new List<IExp>()
                        {
                            new NotExp(new PredicateExp("pred1", new List<NameExp>(){ new NameExp("?a") }))
                        })),
                    new ActionDecl("act1",
                        new ParameterExp(new List<NameExp>(){ new NameExp("?b") }),
                        new AndExp(new List<IExp>()
                        {
                            new PredicateExp("pred1", new List<NameExp>(){ new NameExp("?b") })
                        }),
                        new AndExp(new List<IExp>()
                        {
                            new NotExp(new PredicateExp("pred1", new List<NameExp>(){ new NameExp("?b") }))
                        })),
                },
                2
            };
            yield return new object[] {
                new List<ActionDecl>()
                {
                    new ActionDecl("act1",
                        new ParameterExp(new List<NameExp>(){ new NameExp("?a") }),
                        new AndExp(new List<IExp>()
                        {
                            new PredicateExp("pred1", new List<NameExp>(){ new NameExp("?a") })
                        }),
                        new AndExp(new List<IExp>()
                        {
                            new NotExp(new PredicateExp("pred1", new List<NameExp>(){ new NameExp("?a") }))
                        })),
                    new ActionDecl("act2",
                        new ParameterExp(new List<NameExp>(){ new NameExp("?a") }),
                        new AndExp(new List<IExp>()
                        {
                            new NotExp(new PredicateExp("pred1", new List<NameExp>(){ new NameExp("?a") }))
                        }),
                        new AndExp(new List<IExp>()
                        {
                            new PredicateExp("pred1", new List<NameExp>(){ new NameExp("?a") })
                        })),
                },
                1
            };
            yield return new object[] {
                new List<ActionDecl>()
                {
                    new ActionDecl("act1",
                        new ParameterExp(new List<NameExp>(){ new NameExp("?a") }),
                        new AndExp(new List<IExp>()
                        {
                            new PredicateExp("pred1", new List<NameExp>(){ new NameExp("?a") })
                        }),
                        new AndExp(new List<IExp>()
                        {
                            new NotExp(new PredicateExp("pred1", new List<NameExp>(){ new NameExp("?a") }))
                        })),
                    new ActionDecl("act2",
                        new ParameterExp(new List<NameExp>(){ new NameExp("?a") }),
                        new AndExp(new List<IExp>()
                        {
                            new NotExp(new PredicateExp("pred1", new List<NameExp>(){ new NameExp("?a") }))
                        }),
                        new AndExp(new List<IExp>()
                        {
                            new PredicateExp("pred1", new List<NameExp>(){ new NameExp("?a") })
                        })),
                    new ActionDecl("act3",
                        new ParameterExp(new List<NameExp>(){ new NameExp("?c") }),
                        new AndExp(new List<IExp>()
                        {
                            new PredicateExp("pred1", new List<NameExp>(){ new NameExp("?c") })
                        }),
                        new AndExp(new List<IExp>()
                        {
                            new NotExp(new PredicateExp("pred1", new List<NameExp>(){ new NameExp("?c") }))
                        })),
                },
                2
            };
        }

        [TestMethod]
        [DynamicData(nameof(GetCombineParameters), DynamicDataSourceType.Method)]
        public void Can_Combine_ParameterCount(List<ActionDecl> actions, int expectedParameterCount)
        {
            // ARRANGE
            var combiner = new SimpleActionCombiner();

            // ACT
            var result = combiner.Combine(actions);

            // ASSERT
            Assert.AreEqual(expectedParameterCount, result.Parameters.Values.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Cant_Combine_IfNoActions()
        {
            // ARRANGE
            var combiner = new SimpleActionCombiner();

            // ACT
            var result = combiner.Combine(new List<ActionDecl>());
        }
    }
}
