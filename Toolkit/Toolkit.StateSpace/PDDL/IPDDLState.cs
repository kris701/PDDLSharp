using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Toolkit.StateSpace.PDDL
{
    public interface IPDDLState : IState
    {
        public HashSet<PredicateExp> State { get; set; }
        public PDDLDecl Declaration { get; }

        public IPDDLState Copy();

        public bool Add(PredicateExp pred);
        public bool Add(string pred, params string[] arguments);
        public bool Del(PredicateExp pred);
        public bool Del(string pred, params string[] arguments);
        public bool Contains(PredicateExp pred);
        public bool Contains(string pred, params string[] arguments);

        public int ExecuteNode(INode op);
        public bool IsNodeTrue(INode op);
    }
}
