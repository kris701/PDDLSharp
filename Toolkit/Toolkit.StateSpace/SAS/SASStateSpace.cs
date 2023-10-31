using PDDLSharp.Models.SAS;

namespace PDDLSharp.Toolkit.StateSpace.SAS
{
    public class SASStateSpace : ISASState
    {
        public HashSet<Fact> State { get; set; }
        public SASDecl Declaration { get; }
        public int Count => State.Count;

        public SASStateSpace(SASDecl declaration)
        {
            Declaration = declaration;
            State = new HashSet<Fact>();
            foreach (var fact in declaration.Init)
                State.Add(fact);
        }

        public SASStateSpace(SASDecl declaration, HashSet<Fact> state)
        {
            Declaration = declaration;
            State = state;
        }

        public virtual ISASState Copy()
        {
            var newState = new Fact[State.Count];
            State.CopyTo(newState);
            return new SASStateSpace(Declaration, newState.ToHashSet());
        }

        public bool Add(Fact pred) => State.Add(pred);
        public bool Add(string pred, params string[] arguments) => Add(new Fact(pred, arguments));
        public bool Del(Fact pred) => State.Remove(pred);
        public bool Del(string pred, params string[] arguments) => Del(new Fact(pred, arguments));
        public bool Contains(Fact pred) => State.Contains(pred);
        public bool Contains(string pred, params string[] arguments) => Contains(new Fact(pred, arguments));

        public override bool Equals(object? obj)
        {
            if (obj is ISASState other)
            {
                foreach (var item in State)
                    if (!other.State.Contains(item))
                        return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = State.Count;
            foreach (var item in State)
                hash ^= item.GetHashCode();
            return hash;
        }

        public virtual int ExecuteNode(Operator node)
        {
            int changes = 0;
            foreach (var fact in node.Del)
                if (State.Remove(fact))
                    changes--;
            foreach (var fact in node.Add)
                if (State.Add(fact))
                    changes++;
            return changes;
        }

        public bool IsNodeTrue(Operator node)
        {
            foreach (var fact in node.Pre)
                if (!State.Contains(fact))
                    return false;
            return true;
        }

        public bool IsInGoal()
        {
            foreach (var fact in Declaration.Goal)
                if (!State.Contains(fact))
                    return false;
            return true;
        }
    }
}
