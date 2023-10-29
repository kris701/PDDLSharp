using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.Plans;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.StateSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    public class hDepth : IHeuristic
    {
        public PDDLDecl Declaration { get; }

        public hDepth(PDDLDecl declaration)
        {
            Declaration = declaration;
        }

        public int GetValue(StateMove parent, IState state, List<ActionDecl> groundedActions)
        {
            return parent.hValue - 1;
        }
    }
}
