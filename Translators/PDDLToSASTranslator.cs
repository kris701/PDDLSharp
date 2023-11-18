﻿using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.SAS;
using PDDLSharp.Tools;
using PDDLSharp.Translators.Grounders;
using PDDLSharp.Translators.Tools;
using System;
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
        private HashSet<Fact> _factSet = new HashSet<Fact>();
        private int _factID = 0;
        private int _opID = 0;
        private HashSet<Fact> _negativeFacts = new HashSet<Fact>();
        private string _negatedPrefix = "$neg-";

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
            _factID = 0;
            _factSet = new HashSet<Fact>();
            _opID = 0;
            _negativeFacts = new HashSet<Fact>();

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

            // Handle negative preconditions, if there where any
            if (_negativeFacts.Count > 0)
            {
                var negInits = new HashSet<Fact>();
                // Adds negated facts to all ops
                foreach (var fact in _negativeFacts) 
                {
                    for (int i = 0; i < operators.Count; i++)
                    {
                        var negDels = operators[i].Add.Where(x => x.Name == fact.Name).ToList();
                        var negAdds = operators[i].Del.Where(x => x.Name == fact.Name).ToList();

                        if (negDels.Count == 0 && negAdds.Count == 0)
                            continue;

                        var adds = operators[i].Add.ToHashSet();
                        foreach (var nFact in negAdds)
                        {
                            var negated = GetNegatedOf(nFact);
                            negInits.Add(negated);
                            adds.Add(negated);
                        }
                        var dels = operators[i].Del.ToHashSet();
                        foreach (var nFact in negDels)
                        {
                            var negated = GetNegatedOf(nFact);
                            negInits.Add(negated);
                            dels.Add(negated);
                        }

                        if (adds.Count != operators[i].Add.Count() || dels.Count != operators[i].Del.Count())
                            operators[i] = new Operator(
                                operators[i].Name,
                                operators[i].Arguments,
                                operators[i].Pre,
                                adds.ToArray(),
                                dels.ToArray());
                    }
                    if (Aborted) return new SASDecl();
                }

                // Adds negated facts to init
                foreach (var fact in negInits)
                {
                    bool isTrue = false;
                    foreach(var ini in init)
                    {
                        if (ini.Name == fact.Name.Replace(_negatedPrefix, ""))
                        {
                            isTrue = true;
                            break;
                        }
                    }
                    if (!isTrue)
                        init.Add(fact);
                    if (Aborted) return new SASDecl();
                }
            }

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
                action.Preconditions = EnsureAnd(action.Preconditions);
                action.Effects = EnsureAnd(action.Effects);
                if (Aborted) return new List<Operator>();
                var deconstructedActions = deconstructor.DeconstructAction(action);
                foreach (var deconstructed in deconstructedActions)
                {
                    if (Aborted) return new List<Operator>();
                    var newActs = grounder.Ground(deconstructed).Cast<ActionDecl>();
                    foreach (var act in newActs)
                    {
                        if (Aborted) return new List<Operator>();
                        var args = new List<string>();
                        foreach (var arg in act.Parameters.Values)
                            args.Add(arg.Name);

                        var effFacts = ExtractFactsFromExp(act.Effects, grounder);
                        var add = effFacts[true];
                        var del = effFacts[false];

                        var preFacts = ExtractFactsFromExp(act.Preconditions, grounder);
                        var pre = preFacts[true];
                        if (preFacts[false].Count > 0)
                        {
                            foreach (var fact in preFacts[false])
                            {
                                if (!_negativeFacts.Any(x => x.Name == fact.Name))
                                    _negativeFacts.Add(fact);

                                var nFact = GetNegatedOf(fact);
                                pre.Add(nFact);

                                bool addToAdd = false;
                                bool addToDel = false;
                                if (add.Contains(fact))
                                    addToDel = true;
                                if (del.Contains(fact))
                                    addToAdd = true;

                                if (addToAdd)
                                    add.Add(nFact);
                                if (addToDel)
                                    del.Add(nFact);
                            }
                        }

                        var newOp = new Operator(act.Name, args.ToArray(), pre.ToArray(), add.ToArray(), del.ToArray());
                        newOp.ID = _opID++;
                        operators.Add(newOp);
                    }
                }

            }
            return operators;
        }

        private IExp EnsureAnd(IExp exp)
        {
            if (exp is AndExp)
                return exp;
            return new AndExp(new List<IExp>() { exp });
        }

        private Fact GetFactFromPredicate(PredicateExp pred)
        {
            var name = pred.Name;
            var args = new List<string>();
            foreach (var arg in pred.Arguments)
                args.Add(arg.Name);
            var newFact = new Fact(name, args.ToArray());
            var compound = GetCompound(newFact);
            var find = _factSet.FirstOrDefault(x => GetCompound(x) == compound);
            if (find == null)
            {
                newFact.ID = _factID++;
                _factSet.Add(newFact);
            }
            else
            {
                newFact.ID = find.ID;
            }
            return newFact;
        }

        private Fact GetNegatedOf(Fact fact)
        {
            var newFact = new Fact($"{_negatedPrefix}{fact.Name}", fact.Arguments);
            var compound = GetCompound(newFact);
            var find = _factSet.FirstOrDefault(x => GetCompound(x) == compound);
            if (find == null)
            {
                newFact.ID = _factID++;
                _factSet.Add(newFact);
            }
            else
            {
                newFact.ID = find.ID;
            }
            return newFact;
        }

        private string GetCompound(Fact f)
        {
            var retStr = f.Name;
            foreach (var arg in f.Arguments)
                retStr += arg;
            return retStr;
        }

        private void CheckIfValid(PDDLDecl decl)
        {
            if (decl.Domain.FindTypes<ImplyExp>().Count > 0 || decl.Problem.FindTypes<ImplyExp>().Count > 0)
                throw new Exception("Translator does not support Imply nodes!");
            if (decl.Domain.FindTypes<DerivedDecl>().Count > 0 || decl.Problem.FindTypes<DerivedDecl>().Count > 0)
                throw new Exception("Translator does not support Derived Declaration nodes!");
            if (decl.Domain.FindTypes<TimedLiteralExp>().Count > 0 || decl.Problem.FindTypes<TimedLiteralExp>().Count > 0)
                throw new Exception("Translator does not support Timed Literal nodes!");
            if (decl.Domain.FindTypes<NumericExp>().Count > 0 || decl.Problem.FindTypes<NumericExp>().Count > 0)
                throw new Exception("Translator does not support Numeric nodes!");
            if (decl.Domain.FindTypes<LiteralExp>().Count > 0 || decl.Problem.FindTypes<LiteralExp>().Count > 0)
                throw new Exception("Translator does not support Literal nodes!");
            if (decl.Domain.FindTypes<MetricDecl>().Count > 0 || decl.Problem.FindTypes<MetricDecl>().Count > 0)
                throw new Exception("Translator does not support Metric nodes!");
            if (decl.Domain.FindTypes<SituationDecl>().Count > 0 || decl.Problem.FindTypes<SituationDecl>().Count > 0)
                throw new Exception("Translator does not support Situation nodes!");
            if (decl.Domain.FindTypes<AxiomDecl>().Count > 0 || decl.Problem.FindTypes<AxiomDecl>().Count > 0)
                throw new Exception("Translator does not support Axiom nodes!");
            if (decl.Domain.FindTypes<DurativeActionDecl>().Count > 0 || decl.Problem.FindTypes<DurativeActionDecl>().Count > 0)
                throw new Exception("Translator does not support Durative Actions nodes!");
            if (decl.Domain.FindTypes<ExtendsDecl>().Count > 0 || decl.Problem.FindTypes<ExtendsDecl>().Count > 0)
                throw new Exception("Translator does not support Extends nodes!");
            if (decl.Domain.FindTypes<TimelessDecl>().Count > 0 || decl.Problem.FindTypes<TimelessDecl>().Count > 0)
                throw new Exception("Translator does not support Timeless nodes!");
        }
    }
}
