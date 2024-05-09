using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.PDDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkits
{
    [TestClass]
    public class SimpleStaticPredicateDetectorTests
    {
        [ClassInitialize]
        public static async Task InitialiseAsync(TestContext context)
        {
            var targetPath = "../../../../../Dependencies/downward-benchmarks";
            if (!Directory.Exists(targetPath))
                throw new DirectoryNotFoundException("Benchmarks not found! Please read the readme in the Dependencies folder!");
        }

        [TestMethod]
        [DataRow("../../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "room", "ball", "gripper")]
        [DataRow("../../../../../Dependencies/downward-benchmarks/satellite/domain.pddl", "on_board", "supports", "calibration_target", "satellite", "direction", "instrument", "mode")]
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
