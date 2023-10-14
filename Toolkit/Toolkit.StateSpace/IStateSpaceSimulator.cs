using PDDLSharp.Models;
using PDDLSharp.Models.Plans;

namespace PDDLSharp.Toolkit.StateSpace
{
    public interface IStateSpaceSimulator
    {
        public PDDLDecl Declaration { get; }
        public PDDLStateSpace State { get; }
        public int Cost { get; }

        public void Reset();
        public void Step(string actionName);
        public void Step(string actionName, params string[] arguments);
    }
}
