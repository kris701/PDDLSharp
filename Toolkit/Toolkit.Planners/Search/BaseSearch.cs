using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.StateSpace.SAS;
using System.Diagnostics;
using System.Timers;
using static PDDLSharp.Toolkit.Planners.IPlanner;

namespace PDDLSharp.Toolkit.Planners.Search
{
    public abstract class BaseSearch : IPlanner
    {
        public event LogHandler? OnLog;

        public SASDecl Declaration { get; }
        public int Generated { get; internal set; }
        public int Expanded { get; internal set; }
        public int Evaluations => Heuristic.Evaluations;
        public bool Aborted { get; internal set; }
        public IHeuristic Heuristic { get; }
        public TimeSpan SearchTime { get; internal set; }
        public TimeSpan SearchLimit { get; set; } = TimeSpan.FromMinutes(30);

        internal HashSet<StateMove> _closedList = new HashSet<StateMove>();
        internal RefPriorityQueue _openList = new RefPriorityQueue();
        Stopwatch _watch = new Stopwatch();

        public BaseSearch(SASDecl decl, IHeuristic heuristic)
        {
            Declaration = decl;
            Heuristic = heuristic;
        }

        private System.Timers.Timer GetTimer(TimeSpan interval)
        {
            System.Timers.Timer newTimer = new System.Timers.Timer();
            newTimer.Interval = interval.TotalMilliseconds;
            newTimer.Elapsed += OnTimedOut;
            newTimer.AutoReset = false;
            return newTimer;
        }

        public ActionPlan Solve()
        {
            var logTimer = GetTimer(TimeSpan.FromSeconds(1));
            logTimer.Elapsed -= OnTimedOut;
            logTimer.Elapsed += OnLogStart;
            logTimer.AutoReset = true;
            if (OnLog != null)
                logTimer.Start();

            var timer = GetTimer(SearchLimit);
            timer.Start();
            _watch.Start();

            var state = new SASStateSpace(Declaration);
            if (state.IsInGoal())
                return new ActionPlan(new List<GroundedAction>());

            _closedList = new HashSet<StateMove>();
            _openList = InitializeQueue(Heuristic, state, Declaration.Operators);

            Expanded = 0;
            Generated = 0;

            var result = Solve(Heuristic, state);
            _watch.Stop();
            timer.Stop();
            if (OnLog != null)
                logTimer.Stop();
            SearchTime = _watch.Elapsed;
            return result;
        }

        private void OnTimedOut(object? source, ElapsedEventArgs e)
        {
            Aborted = true;
        }

        private void OnLogStart(object? source, ElapsedEventArgs e)
        {
            SearchTime = _watch.Elapsed;
            if (OnLog != null)
                OnLog.Invoke(this);
        }

        internal ISASState GenerateNewState(ISASState state, Operator op)
        {
            Generated++;
            var newState = state.Copy();
            newState.ExecuteNode(op);
            return newState;
        }

        internal RefPriorityQueue InitializeQueue(IHeuristic h, ISASState state, List<Operator> operators)
        {
            var queue = new RefPriorityQueue();
            var fromMove = new StateMove();
            fromMove.hValue = int.MaxValue;
            var hValue = h.GetValue(fromMove, state, operators);
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

        internal GroundedAction GenerateFromOp(Operator op) => new GroundedAction(op.Name, op.Arguments);

        internal abstract ActionPlan Solve(IHeuristic h, ISASState state);

        public virtual void Dispose()
        {
            _closedList.Clear();
            _closedList.EnsureCapacity(0);
            _openList.Clear();
            _openList.Queue.EnsureCapacity(0);
        }
    }
}
