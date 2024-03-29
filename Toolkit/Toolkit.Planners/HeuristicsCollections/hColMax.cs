﻿using PDDLSharp.Models.SAS;
using PDDLSharp.StateSpaces.SAS;
using PDDLSharp.Toolkit.Planners.Tools;

namespace PDDLSharp.Toolkit.Planners.HeuristicsCollections
{
    /// <summary>
    /// Based on the <seealso href="https://www.fast-downward.org/Doc/Evaluator">max Evaluator</seealso>
    /// </summary>
    public class hColMax : BaseHeuristicCollection
    {
        public hColMax() : base()
        {
        }

        public hColMax(List<IHeuristic> heuristics) : base(heuristics)
        {
        }


        public override int GetValue(StateMove parent, ISASState state, List<Operator> operators)
        {
            Evaluations++;
            int max = -1;
            foreach (var heuristic in Heuristics)
            {
                var hValue = heuristic.GetValue(parent, state, operators);
                if (hValue > max)
                    max = hValue;
            }
            return max;
        }
    }
}
