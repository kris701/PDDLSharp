using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;
using PDDLSharp.StateSpaces.SAS;
using PDDLSharp.Toolkit.Planners.Tools;
using PDDLSharp.Toolkit.Planners.Heuristics;
using PDDLSharp.Toolkit.Planners.Search.Classical;
using PDDLSharp.Toolkit.Planners.HeuristicsCollections;
using PDDLSharp.Tools;
using PDDLSharp.Toolkit.MacroGenerators;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Translators.Grounders;
using PDDLSharp.Translators;

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
        public int SearchBudget { get; set; } = 3;

        private PDDLDecl _pddlDecl;

        public GreedyBFSFocused(PDDLDecl pddlDecl, SASDecl decl, IHeuristic heuristic) : base(decl, heuristic)
        {
            if (heuristic.GetType() != typeof(hGoal))
                throw new Exception("Heuristic must be hGoal!");
            _pddlDecl = pddlDecl.Copy();
        }

        internal override ActionPlan? Solve(IHeuristic h, ISASState state)
        {
            Console.WriteLine($"[{GetPassedTime()}s] Learning Macros...");
            var learnedMacros = LearnFocusedMacros(NumberOfMacros, SearchBudget);
            Console.WriteLine($"[{GetPassedTime()}s] Found {learnedMacros.Count} macros");
            _pddlDecl.Domain.Actions.AddRange(learnedMacros);
            Console.WriteLine($"[{GetPassedTime()}s] Re-translating...");
            var translator = new PDDLToSASTranslator(true);
            translator.TimeLimit = TimeSpan.FromSeconds(1000);
            Declaration = translator.Translate(_pddlDecl);
            state = new SASStateSpace(Declaration);
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
        // Note, the repetition step is left out, since it seemed unnessesary and complicated to make with this system (have to constantly retranslate)
        private List<ActionDecl> LearnFocusedMacros(int nMacros, int budget)
        {
            var newDecl = Declaration.Copy();
            var returnMacros = new List<ActionDecl>();
            var actionCombiner = new SimpleActionCombiner();

            if (Aborted) return new List<ActionDecl>();
            var queue = new FixedMaxPriorityQueue<ActionDecl>(nMacros);
            var h = new EffectHeuristic(new SASStateSpace(newDecl));
            var g = new hPath();

            // Explore state space
            using (var search = new Classical.GreedyBFS(newDecl, new hColSum(new List<IHeuristic>() { g, h })))
            {
                search.SearchLimit = TimeSpan.FromSeconds(budget);
                search.Solve();
                foreach (var state in search._closedList)
                {
                    if (Aborted) return new List<ActionDecl>();
                    if (state.Steps.Count > 0)
                        queue.Enqueue(
                            GenerateMacroFromOperatorSteps(state.Steps),
                            h.GetValue(new StateMove(), state.State, new List<Operator>()));
                }
            }

            // Add unique macros
            int added = 0;
            while (added < nMacros && queue.Count > 0)
            {
                if (Aborted) return new List<ActionDecl>();
                var newMacro = queue.Dequeue();
                if (!returnMacros.Contains(newMacro))
                {
                    returnMacros.Add(newMacro);
                    added++;
                }
            }

            return returnMacros;
        }

        // This section is mostly based on Algorithm 2 from appendix
        // Its quite slow, but this search algorithm is mostly to show the possibilities of reducing generated states, not search time.
        private ActionDecl GenerateMacroFromOperatorSteps(List<Operator> steps)
        {
            var actionCombiner = new SimpleActionCombiner();

            var actionChain = new List<ActionDecl>();
            // Convert operator steps into pddl actions
            foreach (var step in steps)
            {
                var newAct = _pddlDecl.Domain.Actions.First(x => x.Name == step.Name).Copy();
                for (int j = 0; j < newAct.Parameters.Values.Count; j++)
                {
                    var allRefs = newAct.FindNames(newAct.Parameters.Values[j].Name);
                    foreach (var refName in allRefs)
                        refName.Name = $"?{step.Arguments[j]}";
                }
                actionChain.Add(newAct);
            }
            // Generate macro from action chain
            var macro = actionCombiner.Combine(actionChain);
            // Anonymize the parameters of the macro
            AnonymizeActionDecl(macro);
            return macro;
        }

        private void AnonymizeActionDecl(ActionDecl action)
        {
            for (int j = 0; j < action.Parameters.Values.Count; j++)
            {
                var allRefs = action.FindNames(action.Parameters.Values[j].Name);
                foreach (var refName in allRefs)
                    refName.Name = $"?{j}";
            }
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
