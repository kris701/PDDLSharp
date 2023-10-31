﻿using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.PDDL;
using PDDLSharp.Tools;
using PDDLSharp.Translators.StaticPredicateDetectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Translators.Tests.Tools
{
    [TestClass]
    public class SimpleStaticPredicateDetectorTests
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
        [DataRow("benchmarks/gripper/domain.pddl", "room", "ball", "gripper")]
        [DataRow("benchmarks/satellite/domain.pddl", "on_board", "supports", "calibration_target", "satellite", "direction", "instrument", "mode")]
        public void Can_Detect(string domain, params string[] expectedStatics)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<INode> parser = new PDDLParser(listener);
            var decl = new PDDLDecl(
                parser.ParseAs<DomainDecl>(new FileInfo(domain)),
                new ProblemDecl());

            // ACT
            var statics = SimpleStaticPredicateDetector.FindStaticPredicates(decl);

            // ASSERT
            Assert.AreEqual(expectedStatics.Length, statics.Count);
            for (int i = 0; i < expectedStatics.Length; i++)
                Assert.AreEqual(expectedStatics[i], statics[i].Name);
        }
    }
}
