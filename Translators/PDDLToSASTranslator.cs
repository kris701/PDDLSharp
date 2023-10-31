using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.SAS;
using PDDLSharp.Tools;
using PDDLSharp.Translators.Grounders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Linq;

namespace PDDLSharp.Translators
{
    public class PDDLToSASTranslator : ITranslator<PDDLDecl, SASDecl>
    {
        public bool RemoveStaticsFromOperators { get; set; } = false;
        public TimeSpan TimeLimit { get; set; }
        public TimeSpan TranslationTime { get; internal set; }
        public bool Aborted { get; internal set; }
        private ParametizedGrounder _grounder;

        public PDDLToSASTranslator(bool removeStaticsFromOperators)
        {
            RemoveStaticsFromOperators = removeStaticsFromOperators;
        }

        private System.Timers.Timer GetTimer(TimeSpan interval)
        {
            System.Timers.Timer newTimer = new System.Timers.Timer();
            newTimer.Interval = interval.TotalMilliseconds;
            newTimer.Elapsed += OnTimedOut;
            newTimer.AutoReset = false;
            return newTimer;
        }

        private void OnTimedOut(object? source, ElapsedEventArgs e)
        {
            Aborted = true;
            _grounder.Abort();
        }

        public SASDecl Translate(PDDLDecl from)
        {
            Aborted = false;
            var timer = GetTimer(TimeLimit);
            timer.Start();
            var watch = new Stopwatch();
            watch.Start();

            var domainVariables = new HashSet<string>();
            var operators = new List<Operator>();
            var goal = new HashSet<Fact>();
            var init = new HashSet<Fact>();

            _grounder = new ParametizedGrounder(from);
            _grounder.RemoveStaticsFromOutput = RemoveStaticsFromOperators;

            // Domain variables
            if (from.Problem.Objects != null)
                foreach (var obj in from.Problem.Objects.Objs)
                    domainVariables.Add(obj.Name);
            if (from.Domain.Constants != null)
                foreach (var cons in from.Domain.Constants.Constants)
                    domainVariables.Add(cons.Name);

            // Init
            if (from.Problem.Init != null)
                init = ExtractInitFacts(from.Problem.Init.Predicates);
            if (Aborted) return new SASDecl();

            // Goal
            if (from.Problem.Goal != null)
                goal = ExtractFactsFromExp(from.Problem.Goal.GoalExp)[true].Except(init).ToHashSet();
            if (Aborted) return new SASDecl();

            // Operators
            operators = GetOperators(from);
            if (Aborted) return new SASDecl();

            var result = new SASDecl(domainVariables, operators, goal, init);
            watch.Stop();
            timer.Stop();
            TranslationTime = watch.Elapsed;
            return result;
        }

        private Dictionary<bool, HashSet<Fact>> ExtractFactsFromExp(IExp exp, bool possitive = true)
        {
            var facts = new Dictionary<bool, HashSet<Fact>>();
            facts.Add(true, new HashSet<Fact>());
            facts.Add(false, new HashSet<Fact>());

            switch (exp)
            {
                case NumericExp: break;
                case PredicateExp pred: facts[possitive].Add(GetFactFromPredicate(pred)); break;
                case NotExp not: facts = MergeDictionaries(facts, ExtractFactsFromExp(not.Child, !possitive)); break;
                case ForAllExp forAll:
                    var allForalls = _grounder.Ground(forAll).Cast<ForAllExp>();
                    foreach (var all in allForalls)
                        facts = MergeDictionaries(facts, ExtractFactsFromExp(all.Expression, possitive));
                    break;
                case AndExp and:
                    foreach (var child in and.Children)
                        facts = MergeDictionaries(facts, ExtractFactsFromExp(child, possitive));
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

        private List<Operator> GetOperators(PDDLDecl decl)
        {
            var operators = new List<Operator>();
            foreach (var action in decl.Domain.Actions)
            {
                if (action.Preconditions.FindTypes<NotExp>().Count > 0)
                    throw new Exception("Translator does not support negative preconditions!");
                if (action.Effects.FindTypes<IParametized>().Count > 0)
                    throw new Exception("Translator does not IParametized nodes in effects!");

                var newActs = _grounder.Ground(action).Cast<ActionDecl>();
                foreach (var act in newActs)
                {
                    var args = new List<string>();
                    foreach (var arg in act.Parameters.Values)
                        args.Add(arg.Name);

                    var preFacts = ExtractFactsFromExp(act.Preconditions);
                    var pre = preFacts[true];
                    var effFacts = ExtractFactsFromExp(act.Effects);
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
