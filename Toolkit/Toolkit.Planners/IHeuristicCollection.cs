using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners
{
    public interface IHeuristicCollection : IHeuristic
    {
        public List<IHeuristic> Heuristics { get; set; }
    }
}
