using PDDLSharp.Models;
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

        public int GetValue(IState state)
        {
            int cost = 0;
            bool[] covered = new bool[state.Count];
            foreach(var action in Declaration.Domain.Actions)
            {

            }
            return cost;
        }
    }
}
