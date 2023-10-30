using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.StateSpace;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    public class hAdd : BaseHeuristic
    {
        private Dictionary<int, Dictionary<Fact, int>> _graphCache;

        public hAdd(PDDLDecl declaration)
        {
            _graphCache = new Dictionary<int, Dictionary<Fact, int>>();
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

        internal Dictionary<Fact, int> GenerateCostStructure(IState<Fact, Models.SAS.Operator> state, List<Models.SAS.Operator> operators)
        {
            int hash = state.GetHashCode();
            if (_graphCache.ContainsKey(hash))
                return _graphCache[hash];

            state = state.Copy();

            var Ucost = new Dictionary<Operator, int>();
            var dict = new Dictionary<Fact, int>();
            var checkList = new List<KeyValuePair<Fact, int>>();
            var covered = new bool[operators.Count];
            // Add state facts
            foreach (var fact in state.State)
                dict.Add(fact, 0);

            // Add all possible effect facts
            foreach (var act in operators)
                foreach (var fact in act.Add)
                    if (!dict.ContainsKey(fact))
                        dict.Add(fact, int.MaxValue - 1);

            // Foreach applicable grounded action, set their cost to 1
            foreach (var op in operators)
                if (state.IsNodeTrue(op))
                    foreach (var fact in op.Add)
                        dict[fact] = Math.Min(dict[fact], 1);

            // Count all the positive preconditions actions have
            foreach (var op in operators)
                Ucost.Add(op, op.Pre.Length);

            foreach (var item in dict)
                if (item.Value != 0)
                    checkList.Add(item);

            while (!state.IsInGoal())
            {
                var k = checkList.MinBy(x => x.Value);
                state.Add(k.Key);
                checkList.Remove(k);
                for (int i = 0; i < operators.Count; i++)
                {
                    if (!covered[i] && operators[i].Pre.Contains(k.Key))
                    {
                        Ucost[operators[i]]--;
                        if (Ucost[operators[i]] == 0)
                        {
                            covered[i] = true;
                            foreach (var fact in operators[i].Add)
                                dict[fact] = Math.Min(dict[fact], dict[k.Key] + 1);
                        }
                    }
                }
            }

            _graphCache.Add(hash, dict);

            return dict;
        }
    }
}
