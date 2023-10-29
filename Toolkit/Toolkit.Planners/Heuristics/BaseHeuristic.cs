using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.StateSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    public abstract class BaseHeuristic : IHeuristic
    {
        public PDDLDecl Declaration { get; }

        public BaseHeuristic(PDDLDecl declaration)
        {
            Declaration = declaration;
        }

        public abstract int GetValue(StateMove parent, IState state, List<ActionDecl> groundedActions);
    }
}
