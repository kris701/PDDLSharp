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
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TestTools;

namespace PDDLSharp.CodeGenerators.Tests.FastDownward.SAS
{
    [TestClass]
    public class ComplexSASCodeGeneratorTests : BenchmarkBuilder
    {
        public static IEnumerable<object[]> SASData() => GetSASs();

        [TestMethod]
        [DynamicData(nameof(SASData), DynamicDataSourceType.Method)]
        public void Can_Parse_Generate_Parse(string sasFile)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<ISASNode> parser = new FDSASParser(listener);
            var initialDecl = parser.Parse(new FileInfo(sasFile));
            ICodeGenerator<ISASNode> generator = new SASCodeGenerator(listener);

            // ACT
            generator.Generate(initialDecl, "temp.sas");
            var newPlan = parser.Parse(new FileInfo("temp.sas"));

            // ASSERT
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }
    }
}
