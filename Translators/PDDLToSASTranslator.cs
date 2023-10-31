using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.SAS;
using PDDLSharp.Tools;
using PDDLSharp.Translators.Grounders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PDDLSharp.Translators
{
    public class PDDLToSASTranslator : ITranslator<PDDLDecl, SASDecl>
    {
        public SASDecl Translate(PDDLDecl from)
        {
            var domainVariables = new HashSet<string>();
            var operators = new HashSet<Operator>();
            var goal = new HashSet<Fact>();
            var init = new HashSet<Fact>();

            // Domain variables
            if (from.Problem.Objects != null)
                foreach (var obj in from.Problem.Objects.Objs)
                    domainVariables.Add(obj.Name);
            if (from.Domain.Constants != null)
                foreach (var cons in from.Domain.Constants.Constants)
                    domainVariables.Add(cons.Name);

            // Goal
            if (from.Problem.Goal != null)
            {
                var grounder = new ParametizedGrounder(from);
                goal = ExtractFactFromExp(grounder, from.Problem.Goal.GoalExp);
            }

            // Init
            if (from.Problem.Init != null)
                init = ExtractInitFacts(from.Problem.Init.Predicates);

            // Operators
            operators = GetOperators(from);

            return new SASDecl(domainVariables, operators, goal, init);
        }

        private HashSet<Fact> ExtractFactFromExp(IGrounder<IParametized> grounder, IExp exp)
        {
            var goalFacts = new HashSet<Fact>();

            switch (exp)
            {
                case PredicateExp pred: goalFacts.Add(GetFactFromPredicate(pred)); break;
                case ForAllExp forAll:
                    var allForalls = grounder.Ground(forAll).Cast<ForAllExp>();
                    foreach (var all in allForalls)
                        goalFacts.AddRange(ExtractFactFromExp(grounder, all.Expression));
                    break;
                case AndExp and:
                    foreach (var child in and.Children)
                        goalFacts.AddRange(ExtractFactFromExp(grounder, child));
                    break;
                default:
                    throw new ArgumentException($"Cannot translate node type '{exp.GetType().Name}'");
            }

            return goalFacts;
        }

        private HashSet<Fact> ExtractInitFacts(List<IExp> inits)
        {
            var initFacts = new HashSet<Fact>();

            foreach (var init in inits)
                if (init is PredicateExp pred)
                    initFacts.Add(GetFactFromPredicate(pred));

            return initFacts;
        }

        private HashSet<Operator> GetOperators(IGrounder<IParametized> grounder, PDDLDecl decl)
        {
            var operators = new HashSet<Operator>();
            foreach (var action in decl.Domain.Actions)
            {
                action.Preconditions = EnsureAndNode(action.Preconditions);
                action.Effects = EnsureAndNode(action.Effects);
                var newActs = grounder.Ground(action).Cast<Models.PDDL.Domain.ActionDecl>();
                foreach (var newAct in newActs)
                {
                    var args = new List<string>();
                    var pre = new HashSet<Fact>();
                    var add = new HashSet<Fact>();
                    var del = new HashSet<Fact>();

                    operators.Add(new Operator(newAct.Name, args.ToArray(), pre, add, del));
                }
            }
            return operators;
        }

        private Fact GetFactFromPredicate(PredicateExp pred)
        {
            var name = pred.Name;
            var args = new List<string>();
            foreach (var arg in pred.Arguments)
                args.Add(arg.Name);
            return new Fact(name, args.ToArray());
        }
    }
}
