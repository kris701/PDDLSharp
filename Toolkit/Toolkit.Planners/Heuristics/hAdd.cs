using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.StateSpace;
using PDDLSharp.Toolkit.StateSpace.PDDL;
using PDDLSharp.Toolkit.StateSpace.SAS;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    public class hAdd : BaseHeuristic
    {
        private Dictionary<Fact, HashSet<int>> _graphCache;

        public hAdd()
        {
            _graphCache = new Dictionary<Fact, HashSet<int>>();
        }

        public override int GetValue(StateMove parent, IState<Fact, Operator> state, List<Operator> operators)
        {
            Calculated++;
            var cost = 0;
            var dict = GenerateCostStructure(state, operators);
            foreach (var fact in state.Goals)
            {
                var factCost = dict[fact];
                if (factCost == int.MaxValue)
                    return int.MaxValue;
                cost += factCost;
            }
            return cost;
        }

        internal Dictionary<Fact, int> GenerateCostStructure(IState<Fact, Operator> state, List<Models.SAS.Operator> operators)
        {
            var Ucost = new Dictionary<Operator, int>();
            var dict = new Dictionary<Fact, int>();
            var checkList = new PriorityQueue<Fact, int>();
            // Add state facts
            foreach (var fact in state.State)
                dict.Add(fact, 0);

            // Add all possible effect facts
            foreach (var act in operators)
                foreach (var fact in act.Add)
                    if (!dict.ContainsKey(fact))
                        dict.Add(fact, int.MaxValue);

            // Foreach applicable grounded action, set their adds cost to 1
            foreach (var op in operators)
                if (state.IsNodeTrue(op))
                    foreach (var fact in op.Add)
                        dict[fact] = Math.Min(dict[fact], 1);

            // Count all the positive preconditions actions have
            foreach (var op in operators)
            {
                int count = 0;
                foreach (var pre in op.Pre)
                    if (!state.Contains(pre))
                        count++;
                Ucost.Add(op, count);
            }

            foreach (var item in dict)
                checkList.Enqueue(item.Key, item.Value);

            state = state.Copy();

            while (!state.IsInGoal())
            {
                var k = checkList.Dequeue();
                state.Add(k);
                if (_graphCache.ContainsKey(k))
                {
                    foreach(var index in _graphCache[k])
                    {
                        Ucost[operators[index]]--;
                        if (Ucost[operators[index]] == 0)
                        {
                            var opCost = 0;
                            foreach (var fact in operators[index].Add)
                                opCost = AlwaysPossitive(opCost, Math.Min(dict[fact], AlwaysPossitive(dict[k], 1)));
                            foreach (var fact in operators[index].Add)
                                dict[fact] = Math.Min(dict[fact], AlwaysPossitive(dict[k], opCost));
                        }
                    }
                }
                else
                {
                    _graphCache.Add(k, new HashSet<int>());
                    for (int i = 0; i < operators.Count; i++)
                    {
                        if (operators[i].Pre.Contains(k))
                        {
                            _graphCache[k].Add(i);
                            Ucost[operators[i]]--;
                            if (Ucost[operators[i]] == 0)
                            {
                                var opCost = 0;
                                foreach (var fact in operators[i].Pre)
                                    opCost = AlwaysPossitive(opCost, dict[fact]);
                                foreach (var fact in operators[i].Add)
                                    dict[fact] = Math.Min(dict[fact], opCost);
                            }
                        }
                    }
                }
            }

            return dict;
        }

        private int AlwaysPossitive(int value1, int value2)
        {
            if (value1 == int.MaxValue || value2 == int.MaxValue)
                return int.MaxValue;
            return value1 + value2;
        }
    }
}
