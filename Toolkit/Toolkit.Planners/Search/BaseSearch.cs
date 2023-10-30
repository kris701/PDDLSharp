using PDDLSharp.Models;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Toolkit.Grounders;
using PDDLSharp.Toolkit.StateSpace;
using System.Timers;

namespace PDDLSharp.Toolkit.Planners.Search
{
    public abstract class BaseSearch : IPlanner
    {
        public PDDLDecl Declaration { get; }
        public List<ActionDecl> GroundedActions { get; set; }
        public int Generated { get; internal set; }
        public int Expanded { get; internal set; }

        public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(30);

        internal HashSet<StateMove> _closedList = new HashSet<StateMove>();
        internal RefPriorityQueue _openList = new RefPriorityQueue();
        internal bool _abort = false;

        private bool _preprocessed = false;

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

            _closedList = new HashSet<StateMove>();
            _openList = InitializeQueue(h, state);

            Expanded = 0;
            Generated = 0;

            return Solve(h, state);
        }

        private void OnTimedOut(object? source, ElapsedEventArgs e)
        {
            _abort = true;
            throw new Exception("Planner Timed out! Aborting search...");
        }

        internal IState GenerateNewState(IState state, ActionDecl action)
        {
            Generated++;
            var newState = state.Copy();
            newState.ExecuteNode(action.Effects);
            return newState;
        }

        internal RefPriorityQueue InitializeQueue(IHeuristic h, IState state)
        {
            var queue = new RefPriorityQueue();
            var fromMove = new StateMove();
            fromMove.hValue = int.MaxValue;
            var hValue = h.GetValue(fromMove, state, GroundedActions);
            queue.Enqueue(new StateMove(state, hValue), hValue);
            return queue;
        }

        internal StateMove ExpandBestState(RefPriorityQueue? from = null)
        {
            if (from == null)
                from = _openList;
            var stateMove = from.Dequeue();
            _closedList.Add(stateMove);
            Expanded++;
            return stateMove;
        }

        internal abstract ActionPlan Solve(IHeuristic h, IState state);
    }
}
