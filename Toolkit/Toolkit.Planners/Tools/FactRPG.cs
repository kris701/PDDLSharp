﻿using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Planners.Exceptions;
using PDDLSharp.Toolkit.StateSpace.SAS;
using PDDLSharp.Toolkit.StateSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Tools
{
    // Fact Relaxed Planning Graph
    public class FactRPG
    {
        private Dictionary<int, HashSet<Fact>> _stateCache = new Dictionary<int, HashSet<Fact>>();
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
                if (_stateCache.ContainsKey(hash))
                {
                    foreach (var item in _coveredCache[hash])
                        covered[item] = true;

                    if (state.State.Count == _stateCache[hash].Count)
                        throw new RelaxedPlanningGraphException("Actions didnt change the state!");

                    foreach (var fact in _stateCache[hash])
                        if (!dict.ContainsKey(fact))
                            dict.Add(fact, layer);

                    state.State = _stateCache[hash];
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

                    _stateCache.Add(hash, state.State);
                    _coveredCache.Add(hash, newCovers);
                }
                layer++;
            }

            return dict;
        }
    }
}