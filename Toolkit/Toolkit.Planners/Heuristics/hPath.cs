using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.StateSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    /// <summary>
    /// Based on the <seealso href="https://www.fast-downward.org/Doc/Evaluator">g-value Evaluator</seealso>
    /// </summary>
    public class hPath : IHeuristic
    {
        public PDDLDecl Declaration { get; }

        public hPath(PDDLDecl declaration)
        {
            Declaration = declaration;
        }

        public int GetValue(StateMove parent, IState state, List<ActionDecl> groundedActions)
        {
            return parent.Steps.Count + 1;
        }
    }
}
