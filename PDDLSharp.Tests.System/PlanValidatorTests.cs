using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Parsers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.Plans;
using PDDLSharp.Simulators.PlanValidator;
using PDDLSharp.Models;

namespace PDDLSharp.PDDLSharp.Tests.System
{
    [TestClass]
    public class PlanValidatorTests : BasePlanBenchmarkedTests
    {
        [ClassInitialize]
        public static async Task InitialiseAsync(TestContext context)
        {
            await Setup();
        }

        public static IEnumerable<object[]> GetDictionaryData()
        {
            foreach (var key in _testDict.Keys)
            {
                if (_testPlanDict.ContainsKey(new FileInfo(key).Directory.Name))
                    yield return new object[] { key, _testDict[key], _testPlanDict[new FileInfo(key).Directory.Name] };
                else
                    yield return new object[] { key, _testDict[key], new List<string>() };
            }
        }

        [TestMethod]
        [DynamicData(nameof(GetDictionaryData), DynamicDataSourceType.Method)]
        public void Can_ValidatePlans(string domain, List<string> problems, List<string> plans)
        {
            Trace.WriteLine($"Domain: {new FileInfo(domain).Directory.Name}, problems: {problems.Count}, plans: {plans.Count}");

            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<INode> parser = GetParser(domain, listener);
            IParser<ActionPlan> planParser = new FastDownwardPlanParser(listener);
            IPlanValidator validator = new PlanValidator();
            var domainDecl = parser.ParseAs<DomainDecl>(domain);

            // ACT
            bool any = false;
            foreach(var problem in problems)
            {
                var targetPlanStr = new FileInfo(problem).Name.Replace(".pddl", ".plan");
                var targetPlan = plans.FirstOrDefault(x => x.EndsWith(targetPlanStr));
                if (targetPlan != null)
                {
                    Trace.WriteLine($"   Parsing problem: {problem}");
                    var problemDecl = parser.ParseAs<ProblemDecl>(problem);
                    var newDecl = new PDDLDecl(domainDecl, problemDecl);
                    Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
                    listener.Errors.Clear();

                    Trace.WriteLine($"   Parsing plan: {targetPlan}");
                    var plan = planParser.Parse(targetPlan);
                    Assert.IsTrue(validator.Validate(plan, newDecl));
                    any = true;
                }
            }
            //if (!any)
            //    Assert.Inconclusive($"Could not find any plans for the domain+problems!");

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }

        [TestMethod]
        [DynamicData(nameof(GetDictionaryData), DynamicDataSourceType.Method)]
        public void Cant_ValidatePlans_IfIncorrect_AddRandomParts(string domain, List<string> problems, List<string> plans)
        {
            Trace.WriteLine($"Domain: {new FileInfo(domain).Directory.Name}, problems: {problems.Count}, plans: {plans.Count}");

            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<INode> parser = GetParser(domain, listener);
            IParser<ActionPlan> planParser = new FastDownwardPlanParser(listener);
            IPlanValidator validator = new PlanValidator();
            var domainDecl = parser.ParseAs<DomainDecl>(domain);

            // ACT
            bool any = false;
            foreach (var problem in problems)
            {
                var targetPlanStr = new FileInfo(problem).Name.Replace(".pddl", ".plan");
                var targetPlan = plans.FirstOrDefault(x => x.EndsWith(targetPlanStr));
                if (targetPlan != null)
                {
                    Trace.WriteLine($"   Parsing problem: {problem}");
                    var problemDecl = parser.ParseAs<ProblemDecl>(problem);
                    var newDecl = new PDDLDecl(domainDecl, problemDecl);
                    Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
                    listener.Errors.Clear();

                    Trace.WriteLine($"   Parsing plan: {targetPlan}");
                    var plan = planParser.Parse(targetPlan);
                    for (int i = 0; i < plan.Plan.Count; i += 2)
                        plan.Plan.Add(plan.Plan[i]);
                    Assert.IsFalse(validator.Validate(plan, newDecl));
                    any = true;
                }
            }
            if (!any)
                Assert.Inconclusive($"Could not find any plans for the domain+problems!");

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }

        [TestMethod]
        [DynamicData(nameof(GetDictionaryData), DynamicDataSourceType.Method)]
        public void Cant_ValidatePlans_IfIncorrect_RemoveRandomParts(string domain, List<string> problems, List<string> plans)
        {
            Trace.WriteLine($"Domain: {new FileInfo(domain).Directory.Name}, problems: {problems.Count}, plans: {plans.Count}");

            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<INode> parser = GetParser(domain, listener);
            IParser<ActionPlan> planParser = new FastDownwardPlanParser(listener);
            IPlanValidator validator = new PlanValidator();
            var domainDecl = parser.ParseAs<DomainDecl>(domain);

            // ACT
            bool any = false;
            foreach (var problem in problems)
            {
                var targetPlanStr = new FileInfo(problem).Name.Replace(".pddl", ".plan");
                var targetPlan = plans.FirstOrDefault(x => x.EndsWith(targetPlanStr));
                if (targetPlan != null)
                {
                    Trace.WriteLine($"   Parsing problem: {problem}");
                    var problemDecl = parser.ParseAs<ProblemDecl>(problem);
                    var newDecl = new PDDLDecl(domainDecl, problemDecl);
                    Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
                    listener.Errors.Clear();

                    Trace.WriteLine($"   Parsing plan: {targetPlan}");
                    var plan = planParser.Parse(targetPlan);
                    for (int i = 0; i < plan.Plan.Count; i += 2)
                        plan.Plan.RemoveAt(i);
                    Assert.IsFalse(validator.Validate(plan, newDecl));
                    any = true;
                }
            }
            if (!any)
                Assert.Inconclusive($"Could not find any plans for the domain+problems!");

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }

        [TestMethod]
        [DynamicData(nameof(GetDictionaryData), DynamicDataSourceType.Method)]
        public void Cant_ValidatePlans_IfIncorrect_RandomObject(string domain, List<string> problems, List<string> plans)
        {
            Trace.WriteLine($"Domain: {new FileInfo(domain).Directory.Name}, problems: {problems.Count}, plans: {plans.Count}");

            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<INode> parser = GetParser(domain, listener);
            IParser<ActionPlan> planParser = new FastDownwardPlanParser(listener);
            IPlanValidator validator = new PlanValidator();
            var domainDecl = parser.ParseAs<DomainDecl>(domain);

            // ACT
            bool any = false;
            foreach (var problem in problems)
            {
                var targetPlanStr = new FileInfo(problem).Name.Replace(".pddl", ".plan");
                var targetPlan = plans.FirstOrDefault(x => x.EndsWith(targetPlanStr));
                if (targetPlan != null)
                {
                    Trace.WriteLine($"   Parsing problem: {problem}");
                    var problemDecl = parser.ParseAs<ProblemDecl>(problem);
                    var newDecl = new PDDLDecl(domainDecl, problemDecl);
                    Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
                    listener.Errors.Clear();

                    Trace.WriteLine($"   Parsing plan: {targetPlan}");
                    var plan = planParser.Parse(targetPlan);
                    if (plan.Plan.Count > 1)
                    {
                        InsertRandomObjects(plan);
                        Assert.IsFalse(validator.Validate(plan, newDecl));
                        any = true;
                    }
                }
            }
            if (!any)
                Assert.Inconclusive($"Could not find any plans for the domain+problems!");

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }

        private void InsertRandomObjects(ActionPlan plan)
        {
            Random rn = new Random();
            foreach(var act in plan.Plan)
            {
                if (act.Arguments.Count > 0)
                {
                    act.Arguments[0].Name = "not-an-actual-object";
                    break;
                }
            }
        }
    }
}
