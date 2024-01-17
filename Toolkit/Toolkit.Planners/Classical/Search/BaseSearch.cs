using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;
using PDDLSharp.StateSpaces.SAS;
using PDDLSharp.Toolkit.Planners.Tools;
using System.Diagnostics;
using System.Timers;

namespace PDDLSharp.Toolkit.Planners.Classical.Search
{
    public abstract class BaseSearch : IPlanner
    {
        public bool Log { get; set; } = false;
        public SASDecl Declaration { get; }
        public int Generated { get; internal set; }
        public double GeneratedPrSecond => GetItemPrSecond(Generated, _logWatch.Elapsed);
        public int Expanded { get; internal set; }
        public double ExpandedPrSecond => GetItemPrSecond(Expanded, _logWatch.Elapsed);
        public int Evaluations => Heuristic.Evaluations;
        public double EvaluationsPrSecond => GetItemPrSecond(Evaluations, _logWatch.Elapsed);

        public bool Aborted { get; internal set; }
        public IHeuristic Heuristic { get; }
        public TimeSpan SearchTime => _logWatch.Elapsed;
        public TimeSpan SearchLimit { get; set; } = TimeSpan.FromMinutes(30);

        internal HashSet<StateMove> _closedList = new HashSet<StateMove>();
        internal RefPriorityQueue _openList = new RefPriorityQueue();
        private readonly Stopwatch _logWatch = new Stopwatch();
        private System.Timers.Timer _timeoutTimer = new System.Timers.Timer();
        private System.Timers.Timer _logTimer = new System.Timers.Timer();

        private double GetItemPrSecond(int amount, TimeSpan elapsed)
        {
            if (elapsed.TotalMilliseconds == 0)
                return 0;
            return Math.Round(amount / (elapsed.TotalMilliseconds / 1000), 1);
        }

        public BaseSearch(SASDecl decl, IHeuristic heuristic)
        {
            Declaration = decl;
            Heuristic = heuristic;
            SetupTimers();
        }

        private void SetupTimers()
        {
            _timeoutTimer = new System.Timers.Timer();
            _timeoutTimer.Interval = SearchLimit.TotalMilliseconds;
            _timeoutTimer.Elapsed += OnTimedOut;
            _timeoutTimer.AutoReset = false;

            if (Log)
            {
                _logTimer = new System.Timers.Timer();
                _logTimer.Interval = TimeSpan.FromSeconds(1).TotalMilliseconds;
                _logTimer.Elapsed += OnLogTick;
                _logTimer.AutoReset = true;
            }
        }

        public ActionPlan Solve()
        {
            if (Log)
                LogStarted();

            var state = new SASStateSpace(Declaration);
            if (state.IsInGoal())
                return new ActionPlan(new List<GroundedAction>());

            _closedList = new HashSet<StateMove>();
            _openList = InitializeQueue(Heuristic, state, Declaration.Operators);

            Expanded = 0;
            Generated = 0;
            Heuristic.Reset();

            SetupTimers();
            _timeoutTimer.Start();
            _logTimer.Start();
            _logWatch.Start();

            var result = Solve(Heuristic, state);

            _logWatch.Stop();
            _logTimer.Stop();
            _timeoutTimer.Stop();

            if (result == null)
            {
                if (Log)
                    LogFail();
                return new ActionPlan();
            }
            else if (Log)
                LogSuccess(result);

            return result;
        }

        private void OnTimedOut(object? source, ElapsedEventArgs e)
        {
            Aborted = true;
            if (Log)
                LogAbort();
        }

        private void OnLogTick(object? source, ElapsedEventArgs e)
        {
            if (Log)
                LogTick();
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

        internal abstract ActionPlan? Solve(IHeuristic h, ISASState state);

        public virtual void Dispose()
        {
            _closedList.Clear();
            _closedList.EnsureCapacity(0);
            _openList.Clear();
            _openList.Queue.EnsureCapacity(0);

            _logWatch.Stop();
            _logTimer.Stop();
            _timeoutTimer.Stop();

            GC.SuppressFinalize(this);
        }

        internal int GetPassedTime()
        {
            return (int)Math.Round(_logWatch.Elapsed.TotalSeconds, 0);
        }

        public virtual void LogStarted()
        {
            Console.WriteLine($"Planner {GetType().Name} started with heuristic {Heuristic.GetType().Name}");
            Console.WriteLine($"Task has: {Declaration.DomainVariables.Count} domain variables");
            Console.WriteLine($"          {Declaration.Operators.Count} operators");
            Console.WriteLine($"          {Declaration.Init.Count} initial facts");
            Console.WriteLine($"          {Declaration.Goal.Count} goal facts");
            Console.WriteLine($"Solving...");
        }

        public virtual void LogTick()
        {
            Console.WriteLine($"[{GetPassedTime()}s] Expanded {Expanded} ({ExpandedPrSecond}/s). Generated {Generated} ({GeneratedPrSecond}/s). Evaluations {Heuristic.Evaluations} ({EvaluationsPrSecond}/s)");
        }

        public virtual void LogAbort()
        {
            Console.WriteLine($"[{GetPassedTime()}s] Aborting!");
            Console.WriteLine($"[{GetPassedTime()}s] Planner timed out...");
        }

        public virtual void LogFail()
        {
            Console.WriteLine($"[{GetPassedTime()}s] Planner stopped!");
            Console.WriteLine($"[{GetPassedTime()}s] Either no plan was found, or an error occured.");
        }

        public virtual void LogSuccess(ActionPlan plan)
        {
            Console.WriteLine($"[{GetPassedTime()}s] Plan found!");
            Console.WriteLine($"[{GetPassedTime()}s] Plan length: {plan.Plan.Count}, cost {plan.Cost}");
            Console.WriteLine($"[{GetPassedTime()}s] Total Expansions: {Expanded} ({ExpandedPrSecond}/s)");
            Console.WriteLine($"[{GetPassedTime()}s] Total Generated: {Generated} ({GeneratedPrSecond}/s)");
            Console.WriteLine($"[{GetPassedTime()}s] Total Evaluations: {Evaluations} ({EvaluationsPrSecond}/s)");
        }
    }
}
