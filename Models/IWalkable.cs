using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDL.Models
{
    public interface IWalkable : INode, IEnumerable<INode>
    {
    }
}
