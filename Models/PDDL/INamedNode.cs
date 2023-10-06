using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.PDDL
{
    public interface INamedNode : INode
    {
        public string Name { get; set; }
    }
}
