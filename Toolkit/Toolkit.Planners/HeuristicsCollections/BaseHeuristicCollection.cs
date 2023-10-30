﻿using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.StateSpace;

namespace PDDLSharp.Toolkit.Planners.HeuristicsCollections
{
    public abstract class BaseHeuristicCollection : IHeuristicCollection
    {
        public int Calculated { get; internal set; }
        public List<IHeuristic> Heuristics { get; set; }

        public BaseHeuristicCollection(List<IHeuristic> heuristics)
        {
            Heuristics = heuristics;
        }

        public BaseHeuristicCollection()
        {
            Heuristics = new List<IHeuristic>();
        }

        public abstract int GetValue(StateMove parent, IState state, List<ActionDecl> groundedActions);
    }
}
