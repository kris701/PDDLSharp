namespace PDDLSharp.Toolkit.StateSpace
{
    public interface IState
    {
        public int Count { get; }

        public int GetHashCode();
        public bool Equals(object? obj);

        public bool IsInGoal();
    }
}
