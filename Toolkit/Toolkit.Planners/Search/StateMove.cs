using PDDLSharp.Models;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.StateSpace;
using PDDLSharp.Toolkit.StateSpace.SAS;

namespace PDDLSharp.Toolkit.Planners.Search
{
    public class StateMove
    {
        public IState<Fact, Operator> State { get; private set; }
        public List<GroundedAction> Steps { get; set; }
        public int hValue { get; set; }
        public bool Evaluated { get; set; } = true;

        public StateMove(IState<Fact, Operator> state, List<GroundedAction> steps)
        {
            State = state;
            Steps = steps;
            hValue = -1;
        }

        public StateMove(IState<Fact, Operator> state, int hvalue)
        {
            State = state;
            Steps = new List<GroundedAction>();
            hValue = hvalue;
        }

        public StateMove(IState<Fact, Operator> state)
        {
            State = state;
            Steps = new List<GroundedAction>();
            hValue = -1;
        }

        public StateMove()
        {
            State = new SASStateSpace(new PDDLDecl());
            Steps = new List<GroundedAction>();
            hValue = -1;
        }

        private int _hashCache = -1;
        public override int GetHashCode()
        {
            if (_hashCache == -1)
                _hashCache = State.GetHashCode();
            return _hashCache;
        }

        public override bool Equals(object? obj)
        {
            if (obj is StateMove move)
                return move.GetHashCode() == GetHashCode();
            return false;
        }
    }
}
