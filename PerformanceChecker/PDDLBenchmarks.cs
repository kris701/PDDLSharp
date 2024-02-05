using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using PDDLSharp.Analysers;
using PDDLSharp.Analysers.PDDL;
using PDDLSharp.CodeGenerators;
using PDDLSharp.CodeGenerators.PDDL;
using PDDLSharp.Contextualisers;
using PDDLSharp.Contextualisers.PDDL;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.PDDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceChecker
{
    [SimpleJob(RuntimeMoniker.HostProcess)]
    public class PDDLBenchmarks
    {
        public static string _domain = File.ReadAllText("domain.pddl");
        public static string _problem = File.ReadAllText("prob05.pddl");
        private IErrorListener _listener1 = new ErrorListener();
        private IParser<INode> _parser1;
        private IErrorListener _listener2 = new ErrorListener();
        private IParser<INode> _parser2;
        private IErrorListener _listener3 = new ErrorListener();
        private IParser<INode> _parser3;
        private IContextualiser _contextualiser1;
        private PDDLDecl _contextDecl;
        private IErrorListener _listener4 = new ErrorListener();
        private IAnalyser<PDDLDecl> _analyser1;
        private IErrorListener _listener5 = new ErrorListener();
        private ICodeGenerator<INode> _codeGenerator1;
        private IErrorListener _listener6 = new ErrorListener();
        private ICodeGenerator<INode> _codeGenerator2;

        public PDDLBenchmarks()
        {
            _parser1 = new PDDLParser(_listener1);
            _parser2 = new PDDLParser(_listener2);
            _parser3 = new PDDLParser(_listener2);
            _contextualiser1 = new PDDLContextualiser(_listener3);
            _contextDecl = new PDDLDecl(_parser3.ParseAs<DomainDecl>(_domain), _parser3.ParseAs<ProblemDecl>(_problem));
            _analyser1 = new PDDLAnalyser(_listener4);
            _codeGenerator1 = new PDDLCodeGenerator(_listener5);
            _codeGenerator2 = new PDDLCodeGenerator(_listener6);
        }

        [Benchmark]
        public DomainDecl DomainParsing() => _parser2.ParseAs<DomainDecl>(_domain);

        [Benchmark]
        public ProblemDecl ProblemParsing() => _parser1.ParseAs<ProblemDecl>(_problem);

        [Benchmark]
        public void Contextualization() => _contextualiser1.Contexturalise(_contextDecl.Copy());

        [Benchmark]
        public void Analyse() => _analyser1.Analyse(_contextDecl.Copy());

        [Benchmark]
        public string DomainCodeGeneration() => _codeGenerator1.Generate(_contextDecl.Domain);

        [Benchmark]
        public string ProblemCodeGeneration() => _codeGenerator2.Generate(_contextDecl.Problem);
    }
}
