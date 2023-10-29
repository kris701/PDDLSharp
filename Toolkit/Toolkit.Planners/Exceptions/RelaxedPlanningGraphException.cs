using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Exceptions
{
    public class RelaxedPlanningGraphException : Exception
    {
        public RelaxedPlanningGraphException(string? message) : base(message)
        {
        }
    }
}
