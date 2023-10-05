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
        public void Step(string actionName);
        public void Step(string actionName, params string[] arguments);
    }
}
