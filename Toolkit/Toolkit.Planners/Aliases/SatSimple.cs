﻿using PDDLSharp.Models.SAS;
using PDDLSharp.StateSpaces.SAS;
using PDDLSharp.Toolkit.Planners.Heuristics;
using PDDLSharp.Toolkit.Planners.HeuristicsCollections;
using PDDLSharp.Toolkit.Planners.Tools;

namespace PDDLSharp.Toolkit.Planners.Aliases
{
    public class SatSimple : IHeuristic
    {
        public int Evaluations => _inner.Evaluations;
        private readonly IHeuristic _inner;

        public SatSimple(SASDecl decl)
        {
            _inner = new hColSum(new List<IHeuristic>() {
                new hWeighted(new hGoal(), 10000),
                new hFF(decl)
            });
        }

        public int GetValue(StateMove parent, ISASState state, List<Operator> operators) => _inner.GetValue(parent, state, operators);
        public void Reset() => _inner.Reset();
    }
}
