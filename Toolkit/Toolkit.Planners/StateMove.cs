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

        public StateMove(IState state, List<GroundedAction> steps)
        {
            State = state;
            Steps = steps;
        }

        public StateMove(IState state)
        {
            State = state;
            Steps = new List<GroundedAction>();
        }
    }
}
