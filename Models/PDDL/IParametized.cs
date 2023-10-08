using PDDLSharp.Models.PDDL.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.PDDL
{
    public interface IParametized : INode
    {
        public ParameterExp Parameters { get; set; }
    }
}
