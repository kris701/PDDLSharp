using PDDLSharp.Models.PDDL;

namespace PDDLSharp.Toolkit.StateSpace
{
    public interface IState<F, O, D>
    {
        public HashSet<F> State { get; set; }
        public D Declaration { get; }

        public IState<F, O, D> Copy();
        public int Count { get; }

        public bool Add(F pred);
        public bool Add(string pred, params string[] arguments);
        public bool Del(F pred);
        public bool Del(string pred, params string[] arguments);
        public bool Contains(F pred);
        public bool Contains(string pred, params string[] arguments);

        public int GetHashCode();
        public bool Equals(object? obj);

        public int ExecuteNode(O node);
        public bool IsNodeTrue(O node);

        public bool IsInGoal();
    }
}
