using PDDLSharp;
using PDDLSharp.Analysers;
using PDDLSharp.Analysers.PDDL;
using PDDLSharp.CodeGenerators;
using PDDLSharp.CodeGenerators.PDDL;
using PDDLSharp.Contextualisers;
using PDDLSharp.Contextualisers.PDDL;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.PDDL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTools;

namespace PDDLSharp.CodeGenerators.Tests.PDDL
{
    [TestClass]
    public class ComplexPDDLCodeGeneratorTests : BenchmarkBuilder
    {
        public static IEnumerable<object[]> DomainData() => GetDomains();
        public static IEnumerable<object[]> ProblemData() => GetProblems();

        [TestMethod]
        [DynamicData(nameof(DomainData), DynamicDataSourceType.Method)]
        public void Can_Parse_Generate_Parse_Domain(string domain)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<INode> parser = new PDDLParser(listener);
            var initialDecl = parser.ParseAs<DomainDecl>(new FileInfo(domain));
            ICodeGenerator<INode> generator = new PDDLCodeGenerator(listener);

            // ACT
            generator.Generate(initialDecl, "temp.pddl");
            var newDomainDecl = parser.ParseAs<DomainDecl>(new FileInfo("temp.pddl"));

            // ASSERT
            Assert.AreEqual(initialDecl.Name, newDomainDecl.Name);
            Assert.AreEqual(initialDecl.Requirements, newDomainDecl.Requirements);
            Assert.AreEqual(initialDecl.Types, newDomainDecl.Types);
            Assert.AreEqual(initialDecl.Predicates, newDomainDecl.Predicates);
            Assert.AreEqual(initialDecl.Actions.Count, newDomainDecl.Actions.Count);
            for (int i = 0; i < initialDecl.Actions.Count; i++)
                Assert.AreEqual(initialDecl.Actions[i], newDomainDecl.Actions[i]);
            Assert.AreEqual(initialDecl.DurativeActions.Count, newDomainDecl.DurativeActions.Count);
            for (int i = 0; i < initialDecl.DurativeActions.Count; i++)
                Assert.AreEqual(initialDecl.DurativeActions[i], newDomainDecl.DurativeActions[i]);
            Assert.AreEqual(initialDecl.Deriveds.Count, newDomainDecl.Deriveds.Count);
            for (int i = 0; i < initialDecl.Deriveds.Count; i++)
                Assert.AreEqual(initialDecl.Deriveds[i], newDomainDecl.Deriveds[i]);
            Assert.AreEqual(initialDecl.Axioms.Count, newDomainDecl.Axioms.Count);
            for (int i = 0; i < initialDecl.Axioms.Count; i++)
                Assert.AreEqual(initialDecl.Axioms[i], newDomainDecl.Axioms[i]);

            Assert.AreEqual(initialDecl, newDomainDecl);
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }

        [TestMethod]
        [DynamicData(nameof(ProblemData), DynamicDataSourceType.Method)]
        public void Can_Parse_Generate_Parse_Problem(string problem)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<INode> parser = new PDDLParser(listener);
            var initialDecl = parser.ParseAs<ProblemDecl>(new FileInfo(problem));
            ICodeGenerator<INode> generator = new PDDLCodeGenerator(listener);

            // ACT
            generator.Generate(initialDecl, "temp.pddl");
            var newProblemDecl = parser.ParseAs<ProblemDecl>(new FileInfo("temp.pddl"));

            // ASSERT
            Assert.AreEqual(initialDecl.Name, newProblemDecl.Name);
            Assert.AreEqual(initialDecl.DomainName, newProblemDecl.DomainName);
            Assert.AreEqual(initialDecl.Requirements, newProblemDecl.Requirements);
            Assert.AreEqual(initialDecl.Situation, newProblemDecl.Situation);
            Assert.AreEqual(initialDecl.Objects, newProblemDecl.Objects);
            Assert.AreEqual(initialDecl.Init, newProblemDecl.Init);
            Assert.AreEqual(initialDecl.Goal, newProblemDecl.Goal);
            Assert.AreEqual(initialDecl.Metric, newProblemDecl.Metric);

            Assert.AreEqual(initialDecl, newProblemDecl);
            Assert.IsFalse(listener.Errors.Any(x => x.Type == ParseErrorType.Error));
        }
    }
}
