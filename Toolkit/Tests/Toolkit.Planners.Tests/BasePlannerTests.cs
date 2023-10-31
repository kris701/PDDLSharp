using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.SAS;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.PDDL;
using PDDLSharp.Tools;
using PDDLSharp.Translators;
using PDDLSharp.Translators.Grounders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace PDDLSharp.Toolkit.Planners.Tests
{
    public class BasePlannerTests
    {
        private static Dictionary<string, SASDecl> _declCache = new Dictionary<string, SASDecl>();
        internal static SASDecl GetSASDecl(string domain, string problem)
        {
            if (_declCache.ContainsKey(domain + problem))
                return _declCache[domain + problem];

            IErrorListener listener = new ErrorListener();
            IParser<INode> parser = new PDDLParser(listener);
            var pddlDecl = new PDDLDecl(
                parser.ParseAs<DomainDecl>(new FileInfo(domain)),
                parser.ParseAs<ProblemDecl>(new FileInfo(problem))
                );

            ITranslator<PDDLDecl, SASDecl> translator = new PDDLToSASTranslator();
            var decl = translator.Translate(pddlDecl);


            _declCache.Add(domain + problem, decl);
            return decl;
        }

        private static Dictionary<string, PDDLDecl> _pddlDeclCache = new Dictionary<string, PDDLDecl>();
        internal static PDDLDecl GetPDDLDecl(string domain, string problem)
        {
            if (_pddlDeclCache.ContainsKey(domain + problem))
                return _pddlDeclCache[domain + problem];

            IErrorListener listener = new ErrorListener();
            IParser<INode> parser = new PDDLParser(listener);
            var decl = new PDDLDecl(
                parser.ParseAs<DomainDecl>(new FileInfo(domain)),
                parser.ParseAs<ProblemDecl>(new FileInfo(problem))
                );

            _pddlDeclCache.Add(domain + problem, decl);
            return decl;
        }
    }
}
