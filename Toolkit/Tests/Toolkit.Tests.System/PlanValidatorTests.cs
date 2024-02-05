using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.FastDownward.Plans;
using PDDLSharp.Toolkit.PlanValidators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TestTools;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PDDLSharp.Toolit.Tests.System
{
    [TestClass]
    public class PlanValidatorTests : BenchmarkBuilder
    {
        public static IEnumerable<object[]> GetPlanValidationData()
        {
            if (!_isPDDLSetup)
                SetupPDDL();
            if (!_isPlansSetup)
                SetupPlans();
            foreach (var domainFile in _pddlFiles.Keys)
            {
                var domainName = new FileInfo(domainFile).Directory.Name;
                if (!_planFiles.ContainsKey(domainName))
                    continue;
                foreach (var problemFile in _pddlFiles[domainFile])
                {
                    var targetPlanStr = new FileInfo(problemFile).Name.Replace(".pddl", ".plan");
                    var targetPlan = _planFiles[domainName].FirstOrDefault(x => x.EndsWith(targetPlanStr));
                    if (targetPlan != null)
                        yield return new object[] { domainFile, problemFile, targetPlan };
                }
            }
        }

        [TestMethod]
        [DynamicData(nameof(GetPlanValidationData), DynamicDataSourceType.Method)]
        public void Can_ValidatePlans(string domainFile, string problemFile, string planFile)
        {
            Trace.WriteLine($"Domain: {new FileInfo(domainFile).Directory.Name}, problem: {new FileInfo(problemFile).Name}, plan: {new FileInfo(planFile).Name}");

            // ARRANGE
            IPlanValidator validator = new PlanValidator();
            Trace.WriteLine($"   Parsing domain and problem: {problemFile}");
            var newDecl = GetPDDLDecl(domainFile, problemFile);
            Trace.WriteLine($"   Parsing plan: {planFile}");
            var plan = GetActionPlan(planFile);

            // ACT ASSERT
            Assert.IsTrue(validator.Validate(plan, newDecl));
        }

        [TestMethod]
        [DynamicData(nameof(GetPlanValidationData), DynamicDataSourceType.Method)]
        public void Cant_ValidatePlans_IfIncorrect_AddRandomParts(string domainFile, string problemFile, string planFile)
        {
            Trace.WriteLine($"Domain: {new FileInfo(domainFile).Directory.Name}, problem: {new FileInfo(problemFile).Name}, plan: {new FileInfo(planFile).Name}");

            // ARRANGE
            IPlanValidator validator = new PlanValidator();
            Trace.WriteLine($"   Parsing domain and problem: {problemFile}");
            var newDecl = GetPDDLDecl(domainFile, problemFile);
            Trace.WriteLine($"   Parsing plan: {planFile}");
            var plan = GetActionPlan(planFile);

            // ACT ASSERT
            if (plan.Plan.Count > 10)
            {
                int orgSize = plan.Plan.Count;
                for (int i = 0; i < orgSize; i += 2)
                    plan.Plan.Insert(i, plan.Plan[i]);
                Assert.IsFalse(validator.Validate(plan, newDecl));
            }
            else
                Assert.Inconclusive();
        }

        [TestMethod]
        [DynamicData(nameof(GetPlanValidationData), DynamicDataSourceType.Method)]
        public void Cant_ValidatePlans_IfIncorrect_RemoveRandomParts(string domainFile, string problemFile, string planFile)
        {
            Trace.WriteLine($"Domain: {new FileInfo(domainFile).Directory.Name}, problem: {new FileInfo(problemFile).Name}, plan: {new FileInfo(planFile).Name}");

            // ARRANGE
            IPlanValidator validator = new PlanValidator();
            Trace.WriteLine($"   Parsing domain and problem: {problemFile}");
            var newDecl = GetPDDLDecl(domainFile, problemFile);
            Trace.WriteLine($"   Parsing plan: {planFile}");
            var plan = GetActionPlan(planFile);

            // ACT ASSERT
            for (int i = 0; i < plan.Plan.Count; i += 2)
                plan.Plan.RemoveAt(i);
            Assert.IsFalse(validator.Validate(plan, newDecl));
        }

        [TestMethod]
        [DynamicData(nameof(GetPlanValidationData), DynamicDataSourceType.Method)]
        public void Cant_ValidatePlans_IfIncorrect_RandomObject(string domainFile, string problemFile, string planFile)
        {
            Trace.WriteLine($"Domain: {new FileInfo(domainFile).Directory.Name}, problem: {new FileInfo(problemFile).Name}, plan: {new FileInfo(planFile).Name}");

            // ARRANGE
            IPlanValidator validator = new PlanValidator();
            Trace.WriteLine($"   Parsing domain and problem: {problemFile}");
            var newDecl = GetPDDLDecl(domainFile, problemFile);
            Trace.WriteLine($"   Parsing plan: {planFile}");
            var plan = GetActionPlan(planFile);

            // ACT
            if (plan.Plan.Count > 1)
            {
                InsertRandomObjects(plan);
                Assert.IsFalse(validator.Validate(plan, newDecl));
            }
            else
                Assert.Inconclusive();
        }

        private void InsertRandomObjects(ActionPlan plan)
        {
            Random rn = new Random();
            foreach (var act in plan.Plan)
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
