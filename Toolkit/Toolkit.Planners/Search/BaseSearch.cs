using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.Plans;
using PDDLSharp.Models;
using PDDLSharp.Toolkit.Grounders;
using PDDLSharp.Toolkit.StateSpace;
using PDDLSharp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Toolkit.Planners.Search
{
    public abstract class BaseSearch : IPlanner
    {
        public PDDLDecl Declaration { get; }
        public List<ActionDecl> GroundedActions { get; set; }
        public int Generated { get; internal set; }
        public int Expanded { get; internal set; }

        public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(30);

        private bool _preprocessed = false;
        internal bool _abort = false;

        public BaseSearch(PDDLDecl decl)
        {
            Declaration = decl;
            GroundedActions = new List<ActionDecl>();
        }

        public void PreProcess()
        {
            if (_preprocessed)
                return;
            var grounder = new ParametizedGrounder(Declaration);
            grounder.RemoveStaticsFromOutput = true;
            GroundedActions = new List<ActionDecl>();
            foreach (var action in Declaration.Domain.Actions)
            {
                action.Preconditions = EnsureAndNode(action.Preconditions);
                action.Effects = EnsureAndNode(action.Effects);
                GroundedActions.AddRange(grounder.Ground(action).Cast<ActionDecl>());
            }
            _preprocessed = true;
        }

        private IExp EnsureAndNode(IExp from)
        {
            if (from is AndExp)
                return from;
            return new AndExp(new List<IExp>() { from });
        }

        public ActionPlan Solve(IHeuristic h)
        {
            IState state = new PDDLStateSpace(Declaration);
            var timeoutTimer = new System.Timers.Timer();
            timeoutTimer.Interval = Timeout.TotalMilliseconds;
            timeoutTimer.Elapsed += OnTimedOut;
            timeoutTimer.AutoReset = false;
            timeoutTimer.Start();

            return Solve(h, state);
        }

        private void OnTimedOut(object? source, ElapsedEventArgs e)
        {
            _abort = true;
            throw new Exception("Planner Timed out! Aborting search...");
        }

        internal abstract ActionPlan Solve(IHeuristic h, IState state);
    }
}
