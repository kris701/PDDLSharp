using PDDLSharp.Models;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Grounders;
using PDDLSharp.Toolkit.StateSpace;
using PDDLSharp.Toolkit.StateSpace.SAS;
using System.Timers;

namespace PDDLSharp.Toolkit.Planners.Search
{
    public abstract class BaseSearch : IPlanner
    {
        public PDDLDecl Declaration { get; }
        public List<Models.SAS.Operator> Operators { get; internal set; }
        public int Generated { get; internal set; }
        public int Expanded { get; internal set; }
        public int Evaluations => Heuristic.Evaluations;
        public IHeuristic Heuristic { get; }

        public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(30);

        internal HashSet<StateMove> _closedList = new HashSet<StateMove>();
        internal RefPriorityQueue _openList = new RefPriorityQueue();
        internal bool _abort = false;

        private bool _preprocessed = false;

        public BaseSearch(PDDLDecl decl, IHeuristic heuristic)
        {
            Declaration = decl;
            Heuristic = heuristic;
            Operators = new List<Operator>();
        }

        public void PreProcess()
        {
            if (_preprocessed)
                return;
            var grounder = new ParametizedGrounder(Declaration);
            grounder.RemoveStaticsFromOutput = true;
            Operators = new List<Models.SAS.Operator>();
            foreach (var action in Declaration.Domain.Actions)
            {
                action.Preconditions = EnsureAndNode(action.Preconditions);
                action.Effects = EnsureAndNode(action.Effects);
                var newActs = grounder.Ground(action).Cast<Models.PDDL.Domain.ActionDecl>();
                foreach (var newAct in newActs)
                    Operators.Add(new Models.SAS.Operator(newAct));
            }
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

            var state = new SASStateSpace(Declaration);
            var timeoutTimer = new System.Timers.Timer();
            timeoutTimer.Interval = Timeout.TotalMilliseconds;
            timeoutTimer.Elapsed += OnTimedOut;
            timeoutTimer.AutoReset = false;
            timeoutTimer.Start();

            _closedList = new HashSet<StateMove>();
            _openList = InitializeQueue(Heuristic, state);

            Expanded = 0;
            Generated = 0;

            return Solve(Heuristic, state);
        }

        private void OnTimedOut(object? source, ElapsedEventArgs e)
        {
            _abort = true;
            throw new Exception("Planner Timed out! Aborting search...");
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
            _openList.Clear();
            Operators.Clear();
        }
    }
}
