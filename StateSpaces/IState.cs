namespace PDDLSharp.StateSpaces
{
    public interface IState
    {
        public int Count { get; }

        public int GetHashCode();
        public bool Equals(object? obj);

        public bool IsInGoal();
    }
}
