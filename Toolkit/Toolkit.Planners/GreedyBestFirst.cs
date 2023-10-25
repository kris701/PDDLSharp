﻿using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.Plans;
using PDDLSharp.Toolkit.Grounders;
using PDDLSharp.Toolkit.StateSpace;

namespace PDDLSharp.Toolkit.Planners
{
    public class GreedyBestFirst : IPlanner
    {
        public DomainDecl Domain { get; }
        public ProblemDecl Problem { get; }
        private bool _preprocessed = false;
        private List<ActionDecl> _groundedActions = new List<ActionDecl>();

        public GreedyBestFirst(DomainDecl domain, ProblemDecl problem)
        {
            Domain = domain;
            Problem = problem;
        }

        public void PreProcess()
        {
            IGrounder<ActionDecl> grounder = new ActionGrounder(new PDDLDecl(Domain, Problem));
            _groundedActions = new List<ActionDecl>();
            foreach (var action in Domain.Actions)
                _groundedActions.AddRange(grounder.Ground(action));
            _preprocessed = true;
        }

        public ActionPlan Solve(IHeuristic h)
        {
            if (!_preprocessed)
                PreProcess();

            IState state = new RelaxedPDDLStateSpace(new PDDLDecl(Domain, Problem));
            List<GroundedAction> actionSteps = new List<GroundedAction>();
            HashSet<IState> closedList = new HashSet<IState>();
            Queue<StateMove> openList = new Queue<StateMove>();
            List<StateMove> goals = new List<StateMove>();
            openList.Enqueue(new StateMove(state));
            while (openList.Count > 0)
            {
                var stateMove = openList.Dequeue();

                int best = int.MaxValue;
                for (int i = 0; i < _groundedActions.Count; i++)
                {
                    if (stateMove.State.IsNodeTrue(_groundedActions[i].Preconditions))
                    {
                        var check = stateMove.State.Copy();
                        check.ExecuteNode(_groundedActions[i].Effects);
                        var value = h.GetValue(check, _groundedActions[i]);
                        if (value <= best && !closedList.Contains(check))
                        {
                            best = value;
                            var newMove = new StateMove(check, new List<GroundedAction>(stateMove.Steps) { new GroundedAction(_groundedActions[i], _groundedActions[i].Parameters.Values) });
                            openList.Enqueue(newMove);
                            if (check.IsInGoal())
                                goals.Add(newMove);
                        }
                        if (!closedList.Contains(check))
                            closedList.Add(check);
                    }
                }
            }

            var bestGoal = goals.OrderBy(x => x.Steps.Count).ToList();

            return new ActionPlan(bestGoal[0].Steps, bestGoal[0].Steps.Count);
        }

        internal class StateMove
        {
            public IState State { get; private set; }
            public List<GroundedAction> Steps { get; private set; }

            public StateMove(IState state, List<GroundedAction> steps)
            {
                State = state;
                Steps = steps;
            }

            public StateMove(IState state)
            {
                State = state;
                Steps = new List<GroundedAction>();
            }
        }
    }
}
