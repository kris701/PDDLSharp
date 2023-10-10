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
                if (_testPlanDict.ContainsKey(new FileInfo(key).Directory.Name))
                    yield return new object[] { key, _testDict[key], _testPlanDict[new FileInfo(key).Directory.Name] };
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

            // ACT
            foreach(var problem in problems)
            {
                Trace.WriteLine($"   Parsing problem: {problem}");
                var domainDecl = parser.ParseAs<DomainDecl>(domain);
                var problemDecl = parser.ParseAs<ProblemDecl>(problem);
                var newDecl = new PDDLDecl(domainDecl, problemDecl);
                Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
                listener.Errors.Clear();

                var targetPlanStr = new FileInfo(problem).Name.Replace(".pddl", ".plan");
                var targetPlan = plans.First(x => x.EndsWith(targetPlanStr));
                Trace.WriteLine($"   Parsing plan: {targetPlan}");
                var plan = planParser.Parse(targetPlan);
                Assert.IsTrue(validator.Validate(plan, newDecl));
            }

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }

        [TestMethod]
        [DynamicData(nameof(GetDictionaryData), DynamicDataSourceType.Method)]
        public void Cant_ValidatePlans_IfIncorrect(string domain, List<string> problems, List<string> plans)
        {
            Trace.WriteLine($"Domain: {new FileInfo(domain).Directory.Name}, problems: {problems.Count}, plans: {plans.Count}");

            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<INode> parser = GetParser(domain, listener);
            IParser<ActionPlan> planParser = new FastDownwardPlanParser(listener);
            IPlanValidator validator = new PlanValidator();

            // ACT
            foreach (var problem in problems)
            {
                Trace.WriteLine($"   Parsing problem: {problem}");
                var domainDecl = parser.ParseAs<DomainDecl>(domain);
                var problemDecl = parser.ParseAs<ProblemDecl>(problem);
                var newDecl = new PDDLDecl(domainDecl, problemDecl);
                Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
                listener.Errors.Clear();

                var targetPlanStr = new FileInfo(problem).Name.Replace(".pddl", ".plan");
                var targetPlan = plans.First(x => x.EndsWith(targetPlanStr));
                Trace.WriteLine($"   Parsing plan: {targetPlan}");
                var plan = planParser.Parse(targetPlan);
                if (plan.Plan.Count > 0)
                    plan.Plan.Add(plan.Plan[0]);
                Assert.IsFalse(validator.Validate(plan, newDecl));
            }

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }
    }
}
