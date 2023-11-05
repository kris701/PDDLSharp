using PDDLSharp.Models.PDDL;
using PDDLSharp.StateSpaces.PDDL;

namespace PDDLSharp.Toolkit.Simulators.PDDL
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
