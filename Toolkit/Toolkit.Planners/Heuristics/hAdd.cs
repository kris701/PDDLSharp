using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Toolkit.Grounders;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.Planners.Tools;
using PDDLSharp.Toolkit.StateSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    public class hAdd : BaseHeuristic
    {
        internal HashSet<PredicateExp> _goalCache;
        private Dictionary<int, Dictionary<PredicateExp, int>> _graphCache;

        public hAdd(PDDLDecl declaration)
        {
            _goalCache = new HashSet<PredicateExp>();
            _graphCache = new Dictionary<int, Dictionary<PredicateExp, int>>();
            GenerateGoalFacts(declaration.Problem);
        }

        private void GenerateGoalFacts(ProblemDecl problem)
        {
            var extracted = new HashSet<PredicateExp>();
            if (problem.Goal != null)
            {
                var allPreds = problem.Goal.GoalExp.FindTypes<PredicateExp>();
                foreach (var pred in allPreds)
                    if (pred.Parent is not NotExp)
                        extracted.Add(pred);
            }
            var simplified = new HashSet<PredicateExp>();
            foreach (var fact in extracted)
                simplified.Add(SimplifyPredicate(fact));

            _goalCache = simplified;
        }

        private PredicateExp SimplifyPredicate(PredicateExp pred)
        {
            var newPred = new PredicateExp(pred.Name);
            foreach (var arg in pred.Arguments)
                newPred.Arguments.Add(new NameExp(arg.Name));
            return newPred;
        }

        public override int GetValue(StateMove parent, IState state, List<ActionDecl> groundedActions)
        {
            Calculated++;
            var cost = 0;
            var dict = GenerateCostStructure(state, groundedActions);
            foreach (var fact in _goalCache) {
                var factCost = dict[fact];
                if (factCost == int.MaxValue)
                    return int.MaxValue;
                cost += factCost;
            }
            return cost;
        }

        internal Dictionary<PredicateExp, int> GenerateCostStructure(IState state, List<ActionDecl> groundedActions)
        {
            int hash = state.GetHashCode();
            if (_graphCache.ContainsKey(hash))
                return _graphCache[hash];

            state = state.Copy();

            var Ucost = new Dictionary<ActionDecl, int>();
            var dict = new Dictionary<PredicateExp, int>();
            var checkList = new List<KeyValuePair<PredicateExp, int>>();
            var covered = new bool[groundedActions.Count]; 
            // Add state facts
            foreach (var fact in state.State)
                dict.Add(fact, 0);

            // Add all possible effect facts
            foreach(var act in groundedActions)
                if (act.Effects is AndExp andEff)
                    foreach(var fact in andEff)
                        if (fact is PredicateExp pred && !dict.ContainsKey(pred))
                            dict.Add(pred, int.MaxValue - 1);

            // Foreach applicable grounded action, set their cost to 1
            foreach (var op in groundedActions)
                if (state.IsNodeTrue(op.Preconditions))
                    if (op.Effects is AndExp effAnd)
                        foreach (var item in effAnd)
                            if (item is PredicateExp pred)
                                dict[pred] = Math.Min(dict[pred], 1);

            // Count all the positive preconditions actions have
            foreach (var op in groundedActions)
                Ucost.Add(op, PositivePreconditionCount(op));

            foreach (var item in dict)
                if (item.Value != 0)
                    checkList.Add(item);

            while (!state.IsInGoal())
            {
                var k = checkList.MinBy(x => x.Value);
                state.Add(k.Key);
                checkList.Remove(k);
                for (int i = 0; i < groundedActions.Count; i++)
                {
                    if (!covered[i] && groundedActions[i].Preconditions is AndExp preAnd && preAnd.Children.Contains(k.Key))
                    {
                        Ucost[groundedActions[i]]--;
                        if (Ucost[groundedActions[i]] == 0)
                        {
                            covered[i] = true;
                            if (groundedActions[i].Effects is AndExp effAnd)
                                foreach (var item in effAnd)
                                    if (item is PredicateExp pred)
                                        dict[pred] = Math.Min(dict[pred], dict[k.Key] + 1);
                        }
                    }
                }
            }

            _graphCache.Add(hash, dict);

            return dict;
        }

        private int PositivePreconditionCount(ActionDecl act)
        {
            int count = 0;
            if (act.Preconditions is AndExp and)
                foreach (var item in and.Children)
                    if (item is PredicateExp)
                        count++;
            return count;
        }
    }
}
