using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.Plans;
using PDDLSharp.Toolkit.StateSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    public class hBlind : IHeuristic
    {
        public PDDLDecl Declaration { get; }

        public hBlind(PDDLDecl declaration)
        {
            Declaration = declaration;
        }

        public int GetValue(int currentValue, IState state, List<ActionDecl> groundedActions)
        {
            return currentValue - 1;
        }
    }
}
