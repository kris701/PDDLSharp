using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Planners.Exceptions;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.Planners.Tools;
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
            var dict = GenerateRelaxedGraph(state, operators);
            foreach (var fact in state.Goals)
            {
                var factCost = dict[fact];
                if (factCost == int.MaxValue)
                    return int.MaxValue;
                cost += factCost;
            }
            return cost;
        }

        private Dictionary<int, List<Operator>> _operatorCache = new Dictionary<int, List<Operator>>();
        private Dictionary<int, List<int>> _coveredCache = new Dictionary<int, List<int>>();
        internal Dictionary<Fact, int> GenerateRelaxedGraph(IState<Fact, Operator> state, List<Models.SAS.Operator> operators)
        {
            if (state is not RelaxedSASStateSpace)
                state = new RelaxedSASStateSpace(state.Declaration, state.State, state.Goals);

            state = state.Copy();
            bool[] covered = new bool[operators.Count];
            var dict = new Dictionary<Fact, int>();
            foreach (var fact in state.State)
                dict.Add(fact, 0);

            int layer = 1;
            while (!state.IsInGoal())
            {
                var hash = state.GetHashCode();
                if (_operatorCache.ContainsKey(hash))
                {
                    foreach (var item in _coveredCache[hash])
                        covered[item] = true;

                    int changed = 0;
                    foreach (var op in _operatorCache[hash])
                    {
                        changed += state.ExecuteNode(op);
                        foreach (var add in op.Add)
                            if (!dict.ContainsKey(add))
                                dict.Add(add, layer);
                    }
                    if (changed == 0)
                        throw new RelaxedPlanningGraphException("Actions didnt change the state!");
                }
                else
                {
                    var newCovers = new List<int>();
                    var apply = new List<Operator>();
                    for (int i = 0; i < covered.Length; i++)
                    {
                        if (!covered[i])
                        {
                            if (state.IsNodeTrue(operators[i]))
                            {
                                covered[i] = true;
                                apply.Add(operators[i]);
                                newCovers.Add(i);
                            }
                        }
                    }
                    if (apply.Count == 0)
                        throw new RelaxedPlanningGraphException("No applicable actions found!");

                    state = state.Copy();
                    int changed = 0;
                    foreach (var op in apply)
                    {
                        changed += state.ExecuteNode(op);
                        foreach (var add in op.Add)
                            if (!dict.ContainsKey(add))
                                dict.Add(add, layer);
                    }
                    if (changed == 0)
                        throw new RelaxedPlanningGraphException("Actions didnt change the state!");
                    _operatorCache.Add(hash, apply);
                    _coveredCache.Add(hash, newCovers);
                }
                layer++;
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
