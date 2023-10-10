using PDDLSharp.Models;
using PDDLSharp.Models.Plans;

namespace PDDLSharp.Simulators.StateSpace
{
    public interface IStateSpaceSimulator
    {
        public PDDLDecl Declaration { get; }
        public StateSpace State { get; }
        public int Cost { get; }

        public void Reset();
        public void Step(string actionName);
        public void Step(string actionName, params string[] arguments);
        public void ExecutePlan(ActionPlan plan);
        public bool IsInGoal();
    }
}
