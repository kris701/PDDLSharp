using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers.PDDL;
using PDDLSharp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Translators.Tests
{
    [TestClass]
    public class PDDLToSASTranslatorTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            if (!Directory.Exists("benchmarks"))
            {
                Console.WriteLine("Fetching benchmarks...");
                GitFetcher.CheckAndDownloadBenchmarksAsync("https://github.com/aibasel/downward-benchmarks", "benchmarks");
            }
        }

        [TestMethod]
        [DataRow("benchmarks/gripper/domain.pddl", "benchmarks/gripper/prob01.pddl")]
        [DataRow("benchmarks/logistics98/domain.pddl", "benchmarks/logistics98/prob01.pddl")]
        [DataRow("benchmarks/satellite/domain.pddl", "benchmarks/satellite/p01-pfile1.pddl")]
        [DataRow("benchmarks/depot/domain.pddl", "benchmarks/depot/p01.pddl")]
        [DataRow("benchmarks/miconic/domain.pddl", "benchmarks/miconic/s1-0.pddl")]
        public void Can_Translate_ExpectedDomainVars(string domain, string problem)
        {
            // ARRANGE
            var listener = new ErrorListener();
            var parser = new PDDLParser(listener);
            var decl = parser.ParseDecl(new FileInfo(domain), new FileInfo(problem));
            var translator = new PDDLToSASTranslator();
            int expected = 0;
            if (decl.Domain.Constants != null)
                expected += decl.Domain.Constants.Constants.Count;
            if (decl.Problem.Objects != null)
                expected += decl.Problem.Objects.Objs.Count;

            // ACT
            var sas = translator.Translate(decl);

            // ASSERT
            Assert.AreEqual(expected, sas.DomainVariables.Count);
        }

        [TestMethod]
        [DataRow("benchmarks/gripper/domain.pddl", "benchmarks/gripper/prob01.pddl", 36)]
        [DataRow("benchmarks/logistics98/domain.pddl", "benchmarks/logistics98/prob01.pddl", 1368)]
        [DataRow("benchmarks/satellite/domain.pddl", "benchmarks/satellite/p01-pfile1.pddl", 59)]
        [DataRow("benchmarks/depot/domain.pddl", "benchmarks/depot/p01.pddl", 270)]
        [DataRow("benchmarks/miconic/domain.pddl", "benchmarks/miconic/s1-0.pddl", 4)]
        public void Can_Translate_ExpectedOperators(string domain, string problem, int expected)
        {
            // ARRANGE
            var listener = new ErrorListener();
            var parser = new PDDLParser(listener);
            var decl = parser.ParseDecl(new FileInfo(domain), new FileInfo(problem));
            var translator = new PDDLToSASTranslator();

            // ACT
            var sas = translator.Translate(decl);

            // ASSERT
            Assert.AreEqual(expected, sas.Operators.Count);
        }

        [TestMethod]
        [DataRow("benchmarks/gripper/domain.pddl", "benchmarks/gripper/prob01.pddl", 4)]
        [DataRow("benchmarks/logistics98/domain.pddl", "benchmarks/logistics98/prob01.pddl", 6)]
        [DataRow("benchmarks/satellite/domain.pddl", "benchmarks/satellite/p01-pfile1.pddl", 3)]
        [DataRow("benchmarks/depot/domain.pddl", "benchmarks/depot/p01.pddl", 2)]
        [DataRow("benchmarks/miconic/domain.pddl", "benchmarks/miconic/s1-0.pddl", 1)]
        public void Can_Translate_ExpectedGoals(string domain, string problem, int expected)
        {
            // ARRANGE
            var listener = new ErrorListener();
            var parser = new PDDLParser(listener);
            var decl = parser.ParseDecl(new FileInfo(domain), new FileInfo(problem));
            var translator = new PDDLToSASTranslator();

            // ACT
            var sas = translator.Translate(decl);

            // ASSERT
            Assert.AreEqual(expected, sas.Goal.Count);
        }

        [TestMethod]
        [DataRow("benchmarks/gripper/domain.pddl", "benchmarks/gripper/prob01.pddl", 15)]
        [DataRow("benchmarks/logistics98/domain.pddl", "benchmarks/logistics98/prob01.pddl", 64)]
        [DataRow("benchmarks/satellite/domain.pddl", "benchmarks/satellite/p01-pfile1.pddl", 17)]
        [DataRow("benchmarks/depot/domain.pddl", "benchmarks/depot/p01.pddl", 36)]
        [DataRow("benchmarks/miconic/domain.pddl", "benchmarks/miconic/s1-0.pddl", 7)]
        public void Can_Translate_ExpectedInits(string domain, string problem, int expected)
        {
            // ARRANGE
            var listener = new ErrorListener();
            var parser = new PDDLParser(listener);
            var decl = parser.ParseDecl(new FileInfo(domain), new FileInfo(problem));
            var translator = new PDDLToSASTranslator();

            // ACT
            var sas = translator.Translate(decl);

            // ASSERT
            Assert.AreEqual(expected, sas.Init.Count);
        }
    }
}
