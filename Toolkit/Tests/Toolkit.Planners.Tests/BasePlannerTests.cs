using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models;
using PDDLSharp.Parsers.PDDL;
using PDDLSharp.Parsers;
using PDDLSharp.Toolkit.Grounders;
using PDDLSharp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Tests
{
    public class BasePlannerTests
    {
        private static Dictionary<string, HashSet<ActionDecl>> _groundedCache = new Dictionary<string, HashSet<ActionDecl>>();
        internal static HashSet<ActionDecl> GetGroundedActions(PDDLDecl decl)
        {
            if (_groundedCache.ContainsKey(decl.Domain.Name.Name + decl.Problem.Name.Name))
                return _groundedCache[decl.Domain.Name.Name + decl.Problem.Name.Name];

            IGrounder<IParametized> grounder = new ParametizedGrounder(decl);
            var actions = new HashSet<ActionDecl>();
            foreach (var act in decl.Domain.Actions)
                actions.AddRange(grounder.Ground(act).Cast<ActionDecl>().ToHashSet());
            _groundedCache.Add(decl.Domain.Name.Name + decl.Problem.Name.Name, actions);
            return actions;
        }

        private static Dictionary<string, PDDLDecl> _declCache = new Dictionary<string, PDDLDecl>();
        internal static PDDLDecl GetPDDLDecl(string domain, string problem)
        {
            if (_declCache.ContainsKey(domain + problem))
                return _declCache[domain + problem].Copy();

            IErrorListener listener = new ErrorListener();
            IParser<INode> parser = new PDDLParser(listener);
            var decl = new PDDLDecl(
                parser.ParseAs<DomainDecl>(new FileInfo(domain)),
                parser.ParseAs<ProblemDecl>(new FileInfo(problem))
                );
            _declCache.Add(domain + problem, decl);
            return decl.Copy();
        }
    }
}
