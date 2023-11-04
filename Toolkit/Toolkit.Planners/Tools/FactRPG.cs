using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.StateSpace.SAS;

namespace PDDLSharp.Toolkit.Planners.Tools
{
    // Fact Relaxed Planning Graph
    public class FactRPG : BaseRPG
    {
        public Dictionary<Fact, int> GenerateRelaxedGraph(ISASState state, List<Operator> operators)
        {
            if (state is not RelaxedSASStateSpace)
                state = new RelaxedSASStateSpace(state.Declaration, state.State);

            state = state.Copy();
            bool[] covered = new bool[operators.Count];
            var dict = new Dictionary<Fact, int>();
            foreach (var fact in state.State)
                dict.Add(fact, 0);

            int layer = 1;
            while (!state.IsInGoal())
            {
                var apply = GetNewApplicableOperators(state, operators, covered);
                if (apply.Count == 0)
                    return dict;

                state = state.Copy();
                int changed = state.State.Count;
                foreach (var op in apply)
                {
                    state.ExecuteNode(op);
                    foreach (var add in op.Add)
                        if (!dict.ContainsKey(add))
                            dict.Add(add, layer);
                }
                if (changed == state.State.Count)
                    return dict;

                layer++;
            }

            return dict;
        }
    }
}
