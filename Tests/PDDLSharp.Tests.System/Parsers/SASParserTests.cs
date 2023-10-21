using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.Plans;
using PDDLSharp.Models.SAS;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.Plans;
using PDDLSharp.Parsers.SAS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.PDDLSharp.Tests.System.Parsers
{
    [TestClass]
    public class SASParserTests : BaseSASBenchmarkedTests
    {
        [ClassInitialize]
        public static async Task InitialiseAsync(TestContext context)
        {
            await Setup();
        }

        public static IEnumerable<object[]> GetDictionaryData()
        {
            foreach (var key in _testSASDict.Keys)
            {
                yield return new object[] { key, _testSASDict[key] };
            }
        }

        [TestMethod]
        [DynamicData(nameof(GetDictionaryData), DynamicDataSourceType.Method)]
        public void Can_ParsePlans(string domain, List<string> sass)
        {
            Trace.WriteLine($"Domain: {new FileInfo(domain).Directory.Name}, SASs: {sass.Count}");

            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<ISASNode> parser = new SASParser(listener);

            // ACT
            foreach (var sas in sass)
            {
                Trace.WriteLine($"   Parsing SAS: {sas}");
                var plan = parser.Parse(new FileInfo(sas));
                Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
                listener.Errors.Clear();
            }

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }
    }
}
