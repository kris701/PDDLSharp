using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.FastDownward.SAS;
using PDDLSharp.Models.FastDownward.SAS;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.FastDownward.Plans;
using PDDLSharp.Parsers.FastDownward.SAS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTools;

namespace PDDLSharp.Parsers.Tests.FastDownward.SAS
{
    [TestClass]
    public class ComplexSASParserTests : BenchmarkBuilder
    {
        public static IEnumerable<object[]> SASData() => GetSASs();

        [TestMethod]
        [DynamicData(nameof(SASData), DynamicDataSourceType.Method)]
        public void Can_ParseSAS(string sasFile)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<ISASNode> parser = new FDSASParser(listener);

            // ACT
            var plan = parser.Parse(new FileInfo(sasFile));

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }
    }
}
