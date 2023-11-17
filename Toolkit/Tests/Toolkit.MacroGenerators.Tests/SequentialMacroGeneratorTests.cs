using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDDLSharp.CodeGenerators;
using PDDLSharp.CodeGenerators.PDDL;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.FastDownward.Plans;
using PDDLSharp.Parsers.PDDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.MacroGenerators.Tests
{
    [TestClass]
    public class SequentialMacroGeneratorTests
    {
        [TestMethod]
        [DataRow("TestData/gripper-domain.pddl", 4, "TestData/gripper/prob01.plan")]
        [DataRow("TestData/gripper-domain.pddl", 17, "TestData/gripper/prob01.plan", "TestData/gripper/prob02.plan")]
        [DataRow("TestData/gripper-domain.pddl", 29, "TestData/gripper/prob01.plan", "TestData/gripper/prob02.plan", "TestData/gripper/prob03.plan")]
        [DataRow("TestData/gripper-domain.pddl", 41, "TestData/gripper/prob01.plan", "TestData/gripper/prob02.plan", "TestData/gripper/prob03.plan", "TestData/gripper/prob04.plan")]
        [DataRow("TestData/gripper-domain.pddl", 53, "TestData/gripper/prob01.plan", "TestData/gripper/prob02.plan", "TestData/gripper/prob03.plan", "TestData/gripper/prob04.plan", "TestData/gripper/prob05.plan")]
        public void Can_GenerateMacros_MacroCount(string domainFile, int expectedMacros, params string[] planFiles)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<INode> parser = new PDDLParser(listener);
            var domain = parser.ParseAs<DomainDecl>(new FileInfo(domainFile));
            IParser<ActionPlan> planParser = new FDPlanParser(listener);
            List<ActionPlan> plans = new List<ActionPlan>();
            foreach (var file in planFiles)
                plans.Add(planParser.Parse(new FileInfo(file)));
            var decl = new PDDLDecl(domain, new ProblemDecl());
            IMacroGenerator<List<ActionPlan>> generator = new SequentialMacroGenerator(decl);

            // ACT
            var macros = generator.FindMacros(plans, 100);

            // ASSERT
            Assert.AreEqual(expectedMacros, macros.Count);
        }

        [TestMethod]
        [DataRow("TestData/gripper-domain.pddl", "TestData/gripper/prob01.plan", "TestData/gripper-expected/expected1.pddl")]
        [DataRow("TestData/gripper-domain.pddl", "TestData/gripper/prob01.plan", "TestData/gripper-expected/expected2.pddl")]
        public void Can_GenerateMacros_ExpectedMacros(string domainFile, string planFile, params string[] expectedMacroFiles)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<INode> parser = new PDDLParser(listener);
            var domain = parser.ParseAs<DomainDecl>(new FileInfo(domainFile));
            IParser<ActionPlan> planParser = new FDPlanParser(listener);
            List<ActionPlan> plans = new List<ActionPlan>() { planParser.Parse(new FileInfo(planFile)) };
            var decl = new PDDLDecl(domain, new ProblemDecl());
            IMacroGenerator<List<ActionPlan>> generator = new SequentialMacroGenerator(decl);

            // ACT
            var macros = generator.FindMacros(plans, 50);

            // ASSERT
            foreach (var expectedMacroFile in expectedMacroFiles)
            {
                var expected = parser.ParseAs<ActionDecl>(new FileInfo(expectedMacroFile));
                Assert.IsTrue(macros.Contains(expected));
            }
        }
    }
}
