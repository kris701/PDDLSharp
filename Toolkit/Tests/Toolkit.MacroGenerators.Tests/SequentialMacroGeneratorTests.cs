using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDDLSharp.CodeGenerators;
using PDDLSharp.CodeGenerators.PDDL;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.Plans;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.PDDL;
using PDDLSharp.Parsers.Plans;
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
        [DataRow("TestData/gripper-domain.pddl", 10, "TestData/gripper/prob01.plan")]
        [DataRow("TestData/gripper-domain.pddl", 10, "TestData/gripper/prob01.plan", "TestData/gripper/prob02.plan")]
        [DataRow("TestData/gripper-domain.pddl", 45, "TestData/gripper/prob01.plan", "TestData/gripper/prob02.plan", "TestData/gripper/prob03.plan")]
        [DataRow("TestData/gripper-domain.pddl", 45, "TestData/gripper/prob01.plan", "TestData/gripper/prob02.plan", "TestData/gripper/prob03.plan", "TestData/gripper/prob04.plan")]
        [DataRow("TestData/gripper-domain.pddl", 45, "TestData/gripper/prob01.plan", "TestData/gripper/prob02.plan", "TestData/gripper/prob03.plan", "TestData/gripper/prob04.plan", "TestData/gripper/prob05.plan")]
        public void Can_GenerateMacros(string domainFile, int expectedMacros, params string[] planFiles)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<INode> parser = new PDDLParser(listener);
            var domain = parser.ParseAs<DomainDecl>(new FileInfo(domainFile));
            IParser<ActionPlan> planParser = new FastDownwardPlanParser(listener);
            List<ActionPlan> plans = new List<ActionPlan>();
            foreach (var file in planFiles)
                plans.Add(planParser.Parse(new FileInfo(file)));
            var decl = new PDDLDecl(domain, new ProblemDecl());
            IMacroGenerator<List<ActionPlan>> generator = new SequentialMacroGenerator(decl);
            generator.MacroLimit = int.MaxValue;

            // ACT
            var macros = generator.FindMacros(plans);

            // ASSERT
            Assert.AreEqual(expectedMacros, macros.Count);
        }
    }
}
