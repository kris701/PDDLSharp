using PDDLSharp.Models;
using PDDLSharp.Models.Plans;
using PDDLSharp.Toolkit.StateSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Search
{
    public class StateMove
    {
        public IState State { get; private set; }
        public List<GroundedAction> Steps { get; set; }
        public int hValue { get; set; }

        public StateMove(IState state, List<GroundedAction> steps)
        {
            State = state;
            Steps = steps;
            hValue = -1;
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
            hValue = -1;
        }

        public StateMove()
        {
            State = new PDDLStateSpace(new PDDLDecl());
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
