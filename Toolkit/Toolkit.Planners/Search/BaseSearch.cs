using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Grounders;
using PDDLSharp.Toolkit.StateSpace;
using PDDLSharp.Toolkit.StateSpace.SAS;
using System.Diagnostics;
using System.Timers;

namespace PDDLSharp.Toolkit.Planners.Search
{
    public abstract class BaseSearch : IPlanner
    {
        public PDDLDecl Declaration { get; }
        public List<Models.SAS.Operator> Operators { get; set; }
        public int Generated { get; internal set; }
        public int Expanded { get; internal set; }
        public int Evaluations => Heuristic.Evaluations;
        public bool Aborted { get; internal set; }
        public IHeuristic Heuristic { get; }
        public TimeSpan SearchTime { get; internal set; }
        public TimeSpan PreprocessTime { get; internal set; }
        public TimeSpan PreprocessLimit { get; set; } = TimeSpan.FromMinutes(30);
        public TimeSpan SearchLimit { get; set; } = TimeSpan.FromMinutes(30);

        internal HashSet<StateMove> _closedList = new HashSet<StateMove>();
        internal RefPriorityQueue _openList = new RefPriorityQueue();

        private bool _preprocessed = false;
        private ParametizedGrounder _grounder;

        public BaseSearch(PDDLDecl decl, IHeuristic heuristic)
        {
            Declaration = decl;
            Heuristic = heuristic;
            Operators = new List<Operator>();
            _grounder = new ParametizedGrounder(decl);
            _grounder.RemoveStaticsFromOutput = true;
        }

        private System.Timers.Timer GetTimer(TimeSpan interval)
        {
            System.Timers.Timer newTimer = new System.Timers.Timer();
            newTimer.Interval = interval.TotalMilliseconds;
            newTimer.Elapsed += OnTimedOut;
            newTimer.AutoReset = false;
            return newTimer;
        }

        public void PreProcess()
        {
            if (_preprocessed)
                return;
            var timer = GetTimer(PreprocessLimit);
            timer.Start();
            var watch = new Stopwatch();
            watch.Start();
            Operators = new List<Models.SAS.Operator>();
            foreach (var action in Declaration.Domain.Actions)
            {
                action.Preconditions = EnsureAndNode(action.Preconditions);
                action.Effects = EnsureAndNode(action.Effects);
                var newActs = _grounder.Ground(action).Cast<Models.PDDL.Domain.ActionDecl>();
                foreach (var newAct in newActs)
                    Operators.Add(new Models.SAS.Operator(newAct));
            }
            watch.Stop();
            timer.Stop();
            PreprocessTime = watch.Elapsed;
            _preprocessed = true;
        }

        private IExp EnsureAndNode(IExp from)
        {
            if (from is AndExp)
                return from;
            return new AndExp(new List<IExp>() { from });
        }

        public ActionPlan Solve()
        {
            if (!_preprocessed)
                PreProcess();

            var timer = GetTimer(SearchLimit);
            timer.Start();
            var watch = new Stopwatch();
            watch.Start();

            var state = new SASStateSpace(Declaration);

            _closedList = new HashSet<StateMove>();
            _openList = InitializeQueue(Heuristic, state);

            Expanded = 0;
            Generated = 0;

            var result = Solve(Heuristic, state);
            watch.Stop();
            timer.Stop();
            SearchTime = watch.Elapsed;
            return result;
        }

        private void OnTimedOut(object? source, ElapsedEventArgs e)
        {
            Aborted = true;
            _grounder.Abort();
        }

        internal IState<Fact, Models.SAS.Operator> GenerateNewState(IState<Fact, Models.SAS.Operator> state, Models.SAS.Operator op)
        {
            Generated++;
            var newState = state.Copy();
            newState.ExecuteNode(op);
            return newState;
        }

        internal RefPriorityQueue InitializeQueue(IHeuristic h, IState<Fact, Models.SAS.Operator> state)
        {
            var queue = new RefPriorityQueue();
            var fromMove = new StateMove();
            fromMove.hValue = int.MaxValue;
            var hValue = h.GetValue(fromMove, state, Operators);
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

        internal GroundedAction GenerateFromOp(Models.SAS.Operator op) => new GroundedAction(op.Name, op.Arguments);

        internal abstract ActionPlan Solve(IHeuristic h, IState<Fact, Models.SAS.Operator> state);

        public virtual void Dispose()
        {
            _closedList.Clear();
            _closedList.EnsureCapacity(0);
            _openList.Clear();
            _openList.Queue.EnsureCapacity(0);
            Operators.Clear();
            Operators.EnsureCapacity(0);
        }
    }
}
