using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.SAS;
using PDDLSharp.Tools;
using PDDLSharp.Translators.Grounders;
using PDDLSharp.Translators.Tools;
using System.Diagnostics;
using System.Timers;

namespace PDDLSharp.Translators
{
    public class PDDLToSASTranslator : ITranslator<PDDLDecl, SASDecl>
    {
        public bool RemoveStaticsFromOutput { get; set; } = false;
        public TimeSpan TimeLimit { get; set; } = TimeSpan.FromMinutes(30);
        public TimeSpan TranslationTime { get; internal set; }
        public bool Aborted { get; internal set; }
        private ParametizedGrounder? _grounder;
        private NodeDeconstructor? _deconstructor;

        public PDDLToSASTranslator(bool removeStaticsFromOutput = false)
        {
            RemoveStaticsFromOutput = removeStaticsFromOutput;
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
            if (_grounder != null)
                _grounder.Abort();
            if (_deconstructor != null)
                _deconstructor.Abort();
        }

        public SASDecl Translate(PDDLDecl from)
        {
            Aborted = true;
            CheckIfValid(from);
            Aborted = false;

            var timer = GetTimer(TimeLimit);
            timer.Start();
            var watch = new Stopwatch();
            watch.Start();

            var domainVariables = new HashSet<string>();
            var operators = new List<Operator>();
            var goal = new HashSet<Fact>();
            var init = new HashSet<Fact>();

            var grounder = new ParametizedGrounder(from);
            _grounder = grounder;
            grounder.RemoveStaticsFromOutput = RemoveStaticsFromOutput;
            var deconstructor = new NodeDeconstructor(grounder);
            _deconstructor = deconstructor;

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
                goal = ExtractFactsFromExp(
                    deconstructor.Deconstruct(from.Problem.Goal.GoalExp),
                    grounder)[true];
            if (Aborted) return new SASDecl();

            // Operators
            operators = GetOperators(from, grounder, deconstructor);
            if (Aborted) return new SASDecl();

            var result = new SASDecl(domainVariables, operators, goal, init);
            watch.Stop();
            timer.Stop();
            TranslationTime = watch.Elapsed;
            return result;
        }

        private Dictionary<bool, HashSet<Fact>> ExtractFactsFromExp(IExp exp, IGrounder<IParametized> grounder, bool possitive = true)
        {
            var facts = new Dictionary<bool, HashSet<Fact>>();
            facts.Add(true, new HashSet<Fact>());
            facts.Add(false, new HashSet<Fact>());

            switch (exp)
            {
                case NumericExp: break;
                case EmptyExp: break;
                case PredicateExp pred: facts[possitive].Add(GetFactFromPredicate(pred)); break;
                case NotExp not: facts = MergeDictionaries(facts, ExtractFactsFromExp(not.Child, grounder, !possitive)); break;
                case AndExp and:
                    foreach (var child in and.Children)
                        facts = MergeDictionaries(facts, ExtractFactsFromExp(child, grounder, possitive));
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

        private List<Operator> GetOperators(PDDLDecl decl, IGrounder<IParametized> grounder, NodeDeconstructor deconstructor)
        {
            var operators = new List<Operator>();
            foreach (var action in decl.Domain.Actions)
            {
                var deconstructedActions = deconstructor.DeconstructAction(action);
                foreach (var deconstructed in deconstructedActions)
                {
                    var newActs = grounder.Ground(deconstructed).Cast<ActionDecl>();
                    foreach (var act in newActs)
                    {
                        var args = new List<string>();
                        foreach (var arg in act.Parameters.Values)
                            args.Add(arg.Name);

                        var preFacts = ExtractFactsFromExp(act.Preconditions, grounder);
                        var pre = preFacts[true];
                        var effFacts = ExtractFactsFromExp(act.Effects, grounder);
                        var add = effFacts[true];
                        var del = effFacts[false];

                        operators.Add(new Operator(act.Name, args.ToArray(), pre.ToArray(), add.ToArray(), del.ToArray()));
                    }
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

        private void CheckIfValid(PDDLDecl decl)
        {
            foreach (var action in decl.Domain.Actions)
            {
                if (action.Preconditions.FindTypes<NotExp>().Count > 0)
                    throw new Exception("Translator does not support negative preconditions!");
                if (action.FindTypes<ImplyExp>().Count > 0)
                    throw new Exception("Translator does not support Imply nodes!");
            }
        }
    }
}
