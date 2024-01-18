using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;
using PDDLSharp.StateSpaces.SAS;
using PDDLSharp.Toolkit.Planners.Tools;
using PDDLSharp.Toolkit.Planners.Heuristics;
using PDDLSharp.Toolkit.Planners.Search.Classical;
using PDDLSharp.Toolkit.Planners.HeuristicsCollections;
using PDDLSharp.Tools;

namespace PDDLSharp.Toolkit.Planners.Search.BlackBox
{
    /// <summary>
    /// Greedy Best First Search with Focused Macros.
    /// (<seealso href="https://arxiv.org/abs/2004.13242">Efficient Black-Box Planning Using Macro-Actions with Focused Effects</seealso>)
    /// Note, this only works with the <seealso cref="hGoal"/> heuristic.
    /// </summary>
    public class GreedyBFSFocused : BaseBlackBoxSearch
    {
        public int NumberOfMacros { get; set; } = 8;
        public int NumberOfRepetitions { get; set; } = 1;
        public int SearchBudget { get; set; } = 3;

        public GreedyBFSFocused(SASDecl decl, IHeuristic heuristic) : base(decl, heuristic)
        {
            if (heuristic.GetType() != typeof(hGoal))
                throw new Exception("Heuristic must be hGoal!");
        }

        internal override ActionPlan? Solve(IHeuristic h, ISASState state)
        {
            Console.WriteLine($"[{GetPassedTime()}s] Learning Macros...");
            var learnedMacros = LearnFocusedMacros(NumberOfMacros, NumberOfRepetitions, SearchBudget);
            Console.WriteLine($"[{GetPassedTime()}s] Found {learnedMacros.Count} macros");
            Declaration.Operators.AddRange(learnedMacros);
            Console.WriteLine($"[{GetPassedTime()}s] Searching...");

            while (!Aborted && _openList.Count > 0)
            {
                var stateMove = ExpandBestState();
                var applicables = GetApplicables(stateMove.State);
                foreach (var op in applicables)
                {
                    if (Aborted) break;
                    var newMove = new StateMove(Simulate(stateMove.State, op));
                    if (newMove.State.IsInGoal())
                        return new ActionPlan(GeneratePlanChain(stateMove.Steps, op));
                    if (!_closedList.Contains(newMove) && !_openList.Contains(newMove))
                    {
                        var value = h.GetValue(stateMove, newMove.State, new List<Operator>());
                        newMove.Steps = new List<Operator>(stateMove.Steps) { Declaration.Operators[op] };
                        newMove.hValue = value;
                        _openList.Enqueue(newMove, value);
                    }
                }
            }
            return null;
        }

        private HashSet<Operator> LearnFocusedMacros(int nMacros, int nRepetitions, int budget)
        {
            var queue = new PriorityQueue<Operator, int>();

            for(int i = 0; i < nRepetitions; i++)
            {
                if (Aborted) return new HashSet<Operator>();
                using (var search = new Classical.GreedyBFS(Declaration, new hColSum(new List<IHeuristic>()
                {
                    new EffectHeuristic(new SASStateSpace(Declaration)),
                    new hPath()
                })))
                {
                    search.SearchLimit = TimeSpan.FromSeconds(budget / nRepetitions);
                    search.Solve();
                    foreach (var state in search._closedList)
                    {
                        if (Aborted) return new HashSet<Operator>();
                        if (state.Steps.Count > 1)
                            queue.Enqueue(GetOperatorFromGroundedActions(state.Steps), state.hValue);
                    }
                }
            }

            var returnMacros = new HashSet<Operator>();
            while(returnMacros.Count < nMacros && queue.Count > 0)
                returnMacros.Add(queue.Dequeue());

            return returnMacros;
        }

        private Operator GetOperatorFromGroundedActions(List<Operator> operators)
        {
            var pre = new HashSet<Fact>();
            var add = new HashSet<Fact>();
            var del = new HashSet<Fact>();

            pre.AddRange(operators[0].Pre.ToHashSet());
            add.AddRange(operators[0].Add.ToHashSet());
            del.AddRange(operators[0].Del.ToHashSet());

            foreach (var op in operators.Skip(1))
            {
                foreach(var precon in op.Pre)
                    if (!add.Contains(precon))
                        pre.Add(precon);

                foreach (var delete in op.Del)
                {
                    if (add.Contains(delete))
                        add.Remove(delete);
                    del.Add(delete);
                }

                foreach (var adding in op.Add)
                {
                    if (del.Contains(adding))
                        del.Remove(adding);
                    add.Add(adding);
                }
            }

            return new Operator("macro", new string[0], pre.ToArray(), add.ToArray(), del.ToArray());
        }

        private class EffectHeuristic : BaseHeuristic
        {
            private ISASState _initial;
            public EffectHeuristic(ISASState initial)
            {
                _initial = initial;
            }

            public override int GetValue(StateMove parent, ISASState state, List<Operator> operators)
            {
                Evaluations++;
                var changed = Math.Abs(state.Count - _initial.Count);
                if (changed > 0)
                    return changed;
                return int.MaxValue;
            }
        }
    }
}
