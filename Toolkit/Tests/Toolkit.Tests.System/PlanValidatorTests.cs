﻿using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.FastDownward.Plans;
using PDDLSharp.Toolkit.PlanValidator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolit.Tests.System
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
            IParser<ActionPlan> planParser = new FDPlanParser(listener);
            IPlanValidator validator = new PlanValidator();

            // ACT
            bool any = false;
            foreach (var problem in problems)
            {
                var targetPlanStr = new FileInfo(problem).Name.Replace(".pddl", ".plan");
                var targetPlan = plans.FirstOrDefault(x => x.EndsWith(targetPlanStr));
                if (targetPlan != null)
                {
                    Trace.WriteLine($"   Parsing problem: {problem}");
                    var newDecl = GetPDDLDecl(domain, problem);
                    Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
                    listener.Errors.Clear();

                    Trace.WriteLine($"   Parsing plan: {targetPlan}");
                    var plan = planParser.Parse(new FileInfo(targetPlan));
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
            IParser<ActionPlan> planParser = new FDPlanParser(listener);
            IPlanValidator validator = new PlanValidator();


            // ACT
            bool any = false;
            foreach (var problem in problems)
            {
                var targetPlanStr = new FileInfo(problem).Name.Replace(".pddl", ".plan");
                var targetPlan = plans.FirstOrDefault(x => x.EndsWith(targetPlanStr));
                if (targetPlan != null)
                {
                    Trace.WriteLine($"   Parsing problem: {problem}");
                    var newDecl = GetPDDLDecl(domain, problem);
                    Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
                    listener.Errors.Clear();

                    Trace.WriteLine($"   Parsing plan: {targetPlan}");
                    var plan = planParser.Parse(new FileInfo(targetPlan));
                    if (plan.Plan.Count > 10)
                    {
                        int orgSize = plan.Plan.Count;
                        for (int i = 0; i < orgSize; i += 2)
                            plan.Plan.Insert(i, plan.Plan[i]);
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

        [TestMethod]
        [DynamicData(nameof(GetDictionaryData), DynamicDataSourceType.Method)]
        public void Cant_ValidatePlans_IfIncorrect_RemoveRandomParts(string domain, List<string> problems, List<string> plans)
        {
            Trace.WriteLine($"Domain: {new FileInfo(domain).Directory.Name}, problems: {problems.Count}, plans: {plans.Count}");

            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<INode> parser = GetParser(domain, listener);
            IParser<ActionPlan> planParser = new FDPlanParser(listener);
            IPlanValidator validator = new PlanValidator();

            // ACT
            bool any = false;
            foreach (var problem in problems)
            {
                var targetPlanStr = new FileInfo(problem).Name.Replace(".pddl", ".plan");
                var targetPlan = plans.FirstOrDefault(x => x.EndsWith(targetPlanStr));
                if (targetPlan != null)
                {
                    Trace.WriteLine($"   Parsing problem: {problem}");
                    var newDecl = GetPDDLDecl(domain, problem);
                    Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
                    listener.Errors.Clear();

                    Trace.WriteLine($"   Parsing plan: {targetPlan}");
                    var plan = planParser.Parse(new FileInfo(targetPlan));
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
            IParser<ActionPlan> planParser = new FDPlanParser(listener);
            IPlanValidator validator = new PlanValidator();

            // ACT
            bool any = false;
            foreach (var problem in problems)
            {
                var targetPlanStr = new FileInfo(problem).Name.Replace(".pddl", ".plan");
                var targetPlan = plans.FirstOrDefault(x => x.EndsWith(targetPlanStr));
                if (targetPlan != null)
                {
                    Trace.WriteLine($"   Parsing problem: {problem}");
                    var newDecl = GetPDDLDecl(domain, problem);
                    Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
                    listener.Errors.Clear();

                    Trace.WriteLine($"   Parsing plan: {targetPlan}");
                    var plan = planParser.Parse(new FileInfo(targetPlan));
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
