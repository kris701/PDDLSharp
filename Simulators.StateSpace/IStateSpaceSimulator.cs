using PDDLSharp.Models;
using PDDLSharp.Models.Plans;
using PDDLSharp.States.PDDL;

namespace PDDLSharp.Simulators.StateSpace
{
    public interface IStateSpaceSimulator
    {
        public PDDLDecl Declaration { get; }
        public IPDDLState State { get; }
        public int Cost { get; }

        public void Reset();
        public void Step(string actionName);
        public void Step(string actionName, params string[] arguments);
    }
}
