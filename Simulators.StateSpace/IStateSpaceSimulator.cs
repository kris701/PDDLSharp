using PDDLSharp.Models;
using PDDLSharp.Models.Expressions;
using PDDLSharp.Models.Plans;
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
        public HashSet<GroundedPredicate> State { get; }
        public int Cost { get; }

        public bool Contains(string op, params string[] arguments);
        public bool Contains(GroundedPredicate op);
        public void Reset();
        public void Step(string actionName);
        public void Step(string actionName, params string[] arguments);
    }
}
