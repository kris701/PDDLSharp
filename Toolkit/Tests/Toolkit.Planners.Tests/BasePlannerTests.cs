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
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Toolkit.Planners.Tests
{
    public class BasePlannerTests
    {
        private static Dictionary<string, List<ActionDecl>> _groundedCache = new Dictionary<string, List<ActionDecl>>();
        internal static List<ActionDecl> GetGroundedActions(PDDLDecl decl)
        {
            if (_groundedCache.ContainsKey(decl.Domain.Name.Name + decl.Problem.Name.Name))
                return _groundedCache[decl.Domain.Name.Name + decl.Problem.Name.Name];

            IGrounder<IParametized> grounder = new ParametizedGrounder(decl);
            var actions = new List<ActionDecl>();
            foreach (var act in decl.Domain.Actions)
            {
                act.Preconditions = EnsureAndNode(act.Preconditions);
                act.Effects = EnsureAndNode(act.Effects);
                actions.AddRange(grounder.Ground(act).Cast<ActionDecl>());
            }
            _groundedCache.Add(decl.Domain.Name.Name + decl.Problem.Name.Name, actions);
            return actions;
        }

        private static IExp EnsureAndNode(IExp from)
        {
            if (from is AndExp)
                return from;
            return new AndExp(new List<IExp>() { from });
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
