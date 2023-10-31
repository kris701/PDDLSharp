using PDDLSharp.Models.SAS;

namespace PDDLSharp.Toolkit.StateSpace.SAS
{
    public interface ISASState : IState
    {
        public HashSet<Fact> State { get; set; }
        public SASDecl Declaration { get; }

        public ISASState Copy();

        public bool Add(Fact fact);
        public bool Add(string fact, params string[] arguments);
        public bool Del(Fact fact);
        public bool Del(string fact, params string[] arguments);
        public bool Contains(Fact fact);
        public bool Contains(string fact, params string[] arguments);

        public int ExecuteNode(Operator op);
        public bool IsNodeTrue(Operator op);
    }
}
