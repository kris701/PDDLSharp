using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDL.Models
{
    public interface INamedNode : INode
    {
        public string Name { get; set; }
    }
}
