using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;
using PDDLSharp.StateSpaces.SAS;

namespace PDDLSharp.Toolkit.Planners.Tools
{
    public class StateMove
    {
        public ISASState State { get; private set; }
        public List<Operator> Steps { get; set; }
        public int hValue { get; set; }
        public bool Evaluated { get; set; } = true;

        public StateMove(ISASState state, List<Operator> steps)
        {
            State = state;
            Steps = steps;
            hValue = -1;
        }

        public StateMove(ISASState state, int hvalue)
        {
            State = state;
            Steps = new List<Operator>();
            hValue = hvalue;
        }

        public StateMove(ISASState state)
        {
            State = state;
            Steps = new List<Operator>();
            hValue = -1;
        }

        public StateMove()
        {
            State = new SASStateSpace(new SASDecl());
            Steps = new List<Operator>();
            hValue = -1;
        }

        public override int GetHashCode()
        {
            return State.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is StateMove move)
                return move.State.Equals(State);
            return false;
        }

        public override string? ToString()
        {
            return $"Steps: {Steps.Count}, h: {hValue}, State Size: {State.Count}";
        }
    }
}
