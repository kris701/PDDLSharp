using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Exceptions
{
    public class NoSolutionFoundException : Exception
    {
        public NoSolutionFoundException() : base("No solution found!")
        {
        }
    }
}
