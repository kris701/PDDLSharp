using PDDLSharp.Models.Plans;
using PDDLSharp.Toolkit.StateSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners
{
    internal class StateMove
    {
        public IState State { get; private set; }
        public List<GroundedAction> Steps { get; private set; }
        public int hValue { get; private set; }

        public StateMove(IState state, List<GroundedAction> steps, int hvalue)
        {
            State = state;
            Steps = steps;
            hValue = hvalue;
        }

        public StateMove(IState state, int hvalue)
        {
            State = state;
            Steps = new List<GroundedAction>();
            hValue = hvalue;
        }

        public StateMove(IState state)
        {
            State = state;
            Steps = new List<GroundedAction>();
            hValue = 0;
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
