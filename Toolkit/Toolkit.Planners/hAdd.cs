using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Toolkit.StateSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners
{
    public class hAdd : IHeuristic
    {
        public PDDLDecl Declaration { get; }

        public hAdd(PDDLDecl declaration)
        {
            Declaration = declaration;
        }

        public int GetValue(IState state, ActionDecl action)
        {
            int cost = 0;

            if (state.Declaration.Problem.Goal != null) 
            {

                return 0;
            }
            return cost;
        }
    }
}
