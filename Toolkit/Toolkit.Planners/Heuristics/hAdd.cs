using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Toolkit.StateSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    public class hAdd : IHeuristic<RelaxedPDDLStateSpace>
    {
        public PDDLDecl Declaration { get; }

        public hAdd(PDDLDecl declaration)
        {
            Declaration = declaration;
        }

        public int GetValue(IState state)
        {
            int cost = 0;


            return cost;
        }
    }
}
