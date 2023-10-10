using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Simulators.MutexDetector;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Problem;

namespace PDDLSharp.Simulators.MutexDetectors.Tests
{
    [TestClass]
    public class SimpleMutexDetectorTests
    {
        private static NotExp PutInNotNode(IExp node)
        {
            var newNode = new NotExp(null);
            newNode.Child = node;
            node.Parent = newNode;
            return newNode;
        }

        public static IEnumerable<object[]> GetDetectMutexData()
        {
            yield return new object[] {
                new List<ActionDecl>()
                {
                    new ActionDecl(
                        "move",
                        new ParameterExp(new List<NameExp>(){
                            new NameExp("?a"),
                            new NameExp("?b")
                        }),
                        new AndExp(new List<IExp>()
                        {
                            new PredicateExp("location", new List<NameExp>(){ new NameExp("?a") }),
                            PutInNotNode(new PredicateExp("location", new List<NameExp>(){ new NameExp("?b") }))
                        }),
                        new AndExp(new List<IExp>()
                        {
                            PutInNotNode(new PredicateExp("location", new List<NameExp>(){ new NameExp("?a") })),
                            new PredicateExp("location", new List<NameExp>(){ new NameExp("?b") })
                        }))
                },
                new List<PredicateExp>()
                {
                    new PredicateExp("location", new List<NameExp>(){ new NameExp("?pos") })
                }
            };

            yield return new object[] {
                new List<ActionDecl>()
                {
                    new ActionDecl(
                        "move",
                        new ParameterExp(new List<NameExp>(){
                            new NameExp("?a"),
                            new NameExp("?b")
                        }),
                        new AndExp(new List<IExp>()
                        {
                            new PredicateExp("is-item", new List<NameExp>(){ new NameExp("?a") }),
                            new PredicateExp("location", new List<NameExp>(){ new NameExp("?a") }),
                            PutInNotNode(new PredicateExp("location", new List<NameExp>(){ new NameExp("?b") }))
                        }),
                        new AndExp(new List<IExp>()
                        {
                            PutInNotNode(new PredicateExp("location", new List<NameExp>(){ new NameExp("?a") })),
                            new PredicateExp("location", new List<NameExp>(){ new NameExp("?b") })
                        }))
                },
                new List<PredicateExp>()
                {
                    new PredicateExp("location", new List<NameExp>(){ new NameExp("?pos") })
                }
            };

            yield return new object[] {
                new List<ActionDecl>()
                {
                    new ActionDecl(
                        "move",
                        new ParameterExp(new List<NameExp>(){
                            new NameExp("?a"),
                            new NameExp("?b")
                        }),
                        new AndExp(new List<IExp>()
                        {
                            new PredicateExp("is-item", new List<NameExp>(){ new NameExp("?a") }),
                            new PredicateExp("location", new List<NameExp>(){ new NameExp("?a") }),
                            PutInNotNode(new PredicateExp("location", new List<NameExp>(){ new NameExp("?b") }))
                        }),
                        new AndExp(new List<IExp>()
                        {
                            PutInNotNode(new PredicateExp("is-item", new List<NameExp>(){ new NameExp("?a") })),
                            PutInNotNode(new PredicateExp("location", new List<NameExp>(){ new NameExp("?a") })),
                            new PredicateExp("location", new List<NameExp>(){ new NameExp("?b") })
                        }))
                },
                new List<PredicateExp>()
                {
                    new PredicateExp("location", new List<NameExp>(){ new NameExp("?pos") }),
                    new PredicateExp("is-item", new List<NameExp>(){ new NameExp("?pos") })
                }
            };

            yield return new object[] {
                new List<ActionDecl>()
                {
                    new ActionDecl(
                        "pick",
                        new ParameterExp(new List<NameExp>(){
                            new NameExp("?a"),
                            new NameExp("?b")
                        }),
                        new AndExp(new List<IExp>()
                        {
                            new PredicateExp("location", new List<NameExp>(){ new NameExp("?a") }),
                            PutInNotNode(new PredicateExp("location", new List<NameExp>(){ new NameExp("?a") }))
                        }),
                        new AndExp(new List<IExp>()
                        {
                            PutInNotNode(new PredicateExp("location", new List<NameExp>(){ new NameExp("?a") })),
                            new PredicateExp("location", new List<NameExp>(){ new NameExp("?b") })
                        }))
                },
                new List<PredicateExp>()
                {
                }
            };
        }

        [TestMethod]
        [DynamicData(nameof(GetDetectMutexData), DynamicDataSourceType.Method)]
        public void Can_DetectMutex(List<ActionDecl> actions, List<PredicateExp> expectedPredicates)
        {
            // ARRANGE
            IMutexDetectors detector = new SimpleMutexDetector();
            var decl = new PDDLDecl(new DomainDecl(), new ProblemDecl());
            decl.Domain.Actions = actions;

            // ACT
            var mutexes = detector.FindMutexes(decl);

            // ASSERT
            Assert.AreEqual(expectedPredicates.Count, mutexes.Count);
            foreach (var expected in expectedPredicates)
                Assert.IsTrue(mutexes.Any(x => x.Name == expected.Name));
        }
    }
}
