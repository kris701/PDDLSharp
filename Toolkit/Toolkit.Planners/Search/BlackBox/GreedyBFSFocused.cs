using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;
using PDDLSharp.StateSpaces.SAS;
using PDDLSharp.Toolkit.Planners.Tools;
using PDDLSharp.Toolkit.Planners.Heuristics;
using PDDLSharp.Toolkit.Planners.Search.Classical;
using PDDLSharp.Toolkit.Planners.HeuristicsCollections;
using PDDLSharp.Tools;
using PDDLSharp.Toolkit.MacroGenerators;

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
        public int SearchBudget { get; set; } = 1;

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

        // Based on Algorithm 1 from the paper
        private List<Operator> LearnFocusedMacros(int nMacros, int nRepetitions, int budget)
        {
            var newDecl = Declaration.Copy();
            var returnMacros = new List<Operator>();
            var operatorCombiner = new SimpleOperatorCombiner();

            for(int i = 0; i < nRepetitions; i++)
            {
                if (Aborted) return new List<Operator>();
                var queue = new FixedMaxPriorityQueue<Operator>(nMacros / nRepetitions);
                var h = new EffectHeuristic(new SASStateSpace(newDecl));
                var g = new hPath();

                using (var search = new Classical.GreedyBFS(newDecl, new hColSum(new List<IHeuristic>() { g, h })))
                {
                    search.SearchLimit = TimeSpan.FromSeconds(budget / nRepetitions);
                    search.Solve();
                    foreach (var state in search._closedList)
                    {
                        if (Aborted) return new List<Operator>();
                        if (state.Steps.Count > 0)
                            queue.Enqueue(
                                operatorCombiner.Combine(state.Steps), 
                                h.GetValue(new StateMove(), state.State, new List<Operator>()));
                    }
                }

                var newMacros = new List<Operator>();
                while (newMacros.Count < nMacros && queue.Count > 0)
                {
                    if (Aborted) return new List<Operator>();
                    var newMacro = queue.Dequeue();
                    if (!newMacros.Any(x => x.ContentEquals(newMacro)))
                        newMacros.Add(newMacro);
                }
                returnMacros.AddRange(newMacros);

                if (i != nRepetitions - 1)
                {
                    newDecl = Declaration.Copy();
                    // Select random state
                }
            }

            return returnMacros;
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
                var changed = 0;
                foreach (var item in _initial.State)
                    if (!state.Contains(item))
                        changed++;
                foreach (var item in state.State)
                    if (!_initial.Contains(item))
                        changed++;
                if (changed > 0)
                    return changed;
                return int.MaxValue;
            }
        }
    }
}
