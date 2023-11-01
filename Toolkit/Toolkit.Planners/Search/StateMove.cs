using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.StateSpace.SAS;

namespace PDDLSharp.Toolkit.Planners.Search
{
    public class StateMove
    {
        public ISASState State { get; private set; }
        public List<GroundedAction> Steps { get; set; }
        public int hValue { get; set; }
        public bool Evaluated { get; set; } = true;

        public StateMove(ISASState state, List<GroundedAction> steps)
        {
            State = state;
            Steps = steps;
            hValue = -1;
        }

        public StateMove(ISASState state, int hvalue)
        {
            State = state;
            Steps = new List<GroundedAction>();
            hValue = hvalue;
        }

        public StateMove(ISASState state)
        {
            State = state;
            Steps = new List<GroundedAction>();
            hValue = -1;
        }

        public StateMove()
        {
            State = new SASStateSpace(new SASDecl());
            Steps = new List<GroundedAction>();
            hValue = -1;
        }

        public override int GetHashCode()
        {
            return State.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is StateMove move)
                return move.GetHashCode() == GetHashCode();
            return false;
        }
    }
}
