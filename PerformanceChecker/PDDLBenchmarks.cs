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
        public static string _domain = "(define (domain d1)\r\n        (:requirements :typing)\r\n        (:types\r\n            type1 type2 - object\r\n        )\r\n        (:predicates\r\n            (pred1 ?obj - type1)\r\n            (pred2 ?obj1 - type1 ?obj2 - type2)\r\n        )\r\n        (:action action1\r\n            :parameters (?p1 - type1 ?p2 - type2)\r\n    :precondition (and)            :effect \r\n                (and\r\n                    (pred2 ?p1 ?p2)\r\n                )\r\n        )\r\n    )";
        public static string _problem = "(define (problem p1)\r\n        (:domain d1)\r\n        (:objects \r\n            obj1 obj2 obj3 - type1\r\n            obj4 obj5 obj6 - type2\r\n        )\r\n        (:init\r\n               (pred1 obj1) (pred1 obj2) (pred1 obj3)\r\n               (pred2 obj1 obj4) (pred2 obj2 obj5) \r\n        )\r\n        (:goal \r\n            (and \r\n                (pred2 obj1 obj5) \r\n                (pred2 obj2 obj6) \r\n                (pred2 obj3 obj4)\r\n            )\r\n        )\r\n    )";
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
            _codeGenerator1 = new PDDLCodeGenerator(_listener4);
            _codeGenerator2 = new PDDLCodeGenerator(_listener5);
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
        public string ProblemCodeGeneration() => _codeGenerator1.Generate(_contextDecl.Problem);
    }
}
