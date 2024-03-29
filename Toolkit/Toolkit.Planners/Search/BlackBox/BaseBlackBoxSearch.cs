﻿using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;
using PDDLSharp.StateSpaces.SAS;
using PDDLSharp.Toolkit.Planners.Heuristics;
using PDDLSharp.Toolkit.Planners.Search.Classical;

namespace PDDLSharp.Toolkit.Planners.Search.BlackBox
{
    public abstract class BaseBlackBoxSearch : BaseClassicalSearch
    {
        public BaseBlackBoxSearch(SASDecl decl, IHeuristic heuristic) : base(decl, heuristic)
        {
            switch (heuristic)
            {
                case hGoal: break;
                default:
                    throw new Exception("Invalid heuristic for black box planner!");
            }
        }

        public List<int> GetApplicables(ISASState state)
        {
            var returnList = new List<int>();
            for (int i = 0; i < Declaration.Operators.Count; i++)
            {
                if (Aborted) break;
                if (state.IsNodeTrue(Declaration.Operators[i]))
                    returnList.Add(i);
            }
            return returnList;
        }

        public ISASState Simulate(ISASState state, int opIndex)
        {
            Generated++;
            var newState = state.Copy();
            newState.ExecuteNode(Declaration.Operators[opIndex]);
            return newState;
        }

        internal List<GroundedAction> GeneratePlanChain(List<Operator> steps, int newOp)
        {
            var chain = new List<GroundedAction>();

            chain.AddRange(GeneratePlanChain(steps));
            chain.Add(GenerateFromOp(newOp));

            return chain;
        }

        internal GroundedAction GenerateFromOp(int opIndex) => new GroundedAction(Declaration.Operators[opIndex].Name, Declaration.Operators[opIndex].Arguments);
    }
}
