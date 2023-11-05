using PDDLSharp.CodeGenerators;
using PDDLSharp.CodeGenerators.Plans;
using PDDLSharp.CodeGenerators.SAS;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.FastDownward.SAS;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.FastDownward.SAS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.PDDLSharp.Tests.System.CodeGenerators
{
    [TestClass]
    public class SASCodeGeneratorTests : BaseSASBenchmarkedTests
    {
        [ClassInitialize]
        public static async Task InitialiseAsync(TestContext context)
        {
            await Setup();
        }

        public static IEnumerable<object[]> GetDictionaryData()
        {
            foreach (var key in _testSASDict.Keys)
                yield return new object[] { key, _testSASDict[key] };
        }

        [TestMethod]
        [DynamicData(nameof(GetDictionaryData), DynamicDataSourceType.Method)]
        public void Can_Parse_Generate_Parse(string domain, List<string> sass)
        {
            Trace.WriteLine($"Domain: {new FileInfo(domain).Directory.Name}, sass: {sass.Count}");

            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<ISASNode> parser = new FDSASParser(listener);
            ICodeGenerator<ISASNode> generator = new SASCodeGenerator(listener);

            // ACT
            foreach (var sas in sass)
            {
                Trace.WriteLine($"Testing sas '{sas}'");
                var orgPlan = parser.Parse(new FileInfo(sas));
                generator.Generate(orgPlan, "temp.sas");
                var newPlan = parser.Parse(new FileInfo("temp.sas"));
                Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
            }

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }
    }
}
