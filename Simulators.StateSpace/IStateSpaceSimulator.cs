using PDDLSharp.Models;
using PDDLSharp.Models.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Simulators.StateSpace
{
    public interface IStateSpaceSimulator
    {
        public PDDLDecl Declaration { get; }
        public HashSet<Operator> State { get; }
        public int Cost { get; }

        public void Reset();
        public void Step(string actionName, List<OperatorObject> arguments);
        public void Step(string actionName, string[] arguments);
    }
}
