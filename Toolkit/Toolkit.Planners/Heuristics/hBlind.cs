using PDDLSharp.Models;
using PDDLSharp.Toolkit.StateSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    public class hBlind : IHeuristic<PDDLStateSpace>
    {
        public PDDLDecl Declaration { get; }

        public hBlind(PDDLDecl declaration)
        {
            Declaration = declaration;
        }

        public int GetValue(IState state)
        {
            return 1;
        }
    }
}
