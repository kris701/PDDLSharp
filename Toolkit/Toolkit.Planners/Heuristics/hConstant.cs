using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models;
using PDDLSharp.Toolkit.StateSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Toolkit.Planners.Search;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    public class hConstant : IHeuristic
    {
        public PDDLDecl Declaration { get; }

        public hConstant(PDDLDecl declaration)
        {
            Declaration = declaration;
        }

        public int GetValue(StateMove parent, IState state, List<ActionDecl> groundedActions)
        {
            return 1;
        }
    }
}
