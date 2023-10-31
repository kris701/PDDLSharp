using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
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
        public bool RemoveStaticsFromOperators { get; set; } = false;

        public SASDecl Translate(PDDLDecl from)
        {
            var domainVariables = new HashSet<string>();
            var operators = new List<Operator>();
            var goal = new HashSet<Fact>();
            var init = new HashSet<Fact>();

            var grounder = new ParametizedGrounder(from);
            grounder.RemoveStaticsFromOutput = RemoveStaticsFromOperators;

            // Domain variables
            if (from.Problem.Objects != null)
                foreach (var obj in from.Problem.Objects.Objs)
                    domainVariables.Add(obj.Name);
            if (from.Domain.Constants != null)
                foreach (var cons in from.Domain.Constants.Constants)
                    domainVariables.Add(cons.Name);

            // Goal
            if (from.Problem.Goal != null)
                goal = ExtractFactsFromExp(grounder, from.Problem.Goal.GoalExp)[true];

            // Init
            if (from.Problem.Init != null)
                init = ExtractInitFacts(from.Problem.Init.Predicates);

            // Operators
            operators = GetOperators(grounder, from).ToList();

            return new SASDecl(domainVariables, operators, goal, init);
        }

        private Dictionary<bool, HashSet<Fact>> ExtractFactsFromExp(IGrounder<IParametized> grounder, IExp exp, bool possitive = true)
        {
            var facts = new Dictionary<bool, HashSet<Fact>>();
            facts.Add(true, new HashSet<Fact>());
            facts.Add(false, new HashSet<Fact>());

            switch (exp)
            {
                case NumericExp num: break;
                case PredicateExp pred: facts[possitive].Add(GetFactFromPredicate(pred)); break;
                case NotExp not: facts = MergeDictionaries(facts, ExtractFactsFromExp(grounder, not.Child, !possitive)); break;
                case ForAllExp forAll:
                    var allForalls = grounder.Ground(forAll).Cast<ForAllExp>();
                    foreach (var all in allForalls)
                        facts = MergeDictionaries(facts, ExtractFactsFromExp(grounder, all.Expression, possitive));
                    break;
                case AndExp and:
                    foreach (var child in and.Children)
                        facts = MergeDictionaries(facts, ExtractFactsFromExp(grounder, child, possitive));
                    break;
                default:
                    throw new ArgumentException($"Cannot translate node type '{exp.GetType().Name}'");
            }

            return facts;
        }

        private Dictionary<bool, HashSet<Fact>> MergeDictionaries(Dictionary<bool, HashSet<Fact>> dict1, Dictionary<bool, HashSet<Fact>> dict2)
        {
            var resultDict = new Dictionary<bool, HashSet<Fact>>();
            foreach (var key in dict1.Keys)
                resultDict.Add(key, dict1[key]);
            foreach (var key in dict2.Keys)
                resultDict[key].AddRange(dict2[key]);

            return resultDict;
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
                var newActs = grounder.Ground(action).Cast<ActionDecl>();
                foreach (var act in newActs)
                {
                    var args = new List<string>();
                    foreach (var arg in act.Parameters.Values)
                        args.Add(arg.Name);

                    var preFacts = ExtractFactsFromExp(grounder, act.Preconditions);
                    var pre = preFacts[true];
                    var effFacts = ExtractFactsFromExp(grounder, act.Effects);
                    var add = effFacts[true];
                    var del = effFacts[false];

                    operators.Add(new Operator(act.Name, args.ToArray(), pre, add, del));
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
