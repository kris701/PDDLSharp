using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLModels
{
    public interface INode
    {
        INode Parent { get; }
        int Start { get; set; }
        int End { get; set; }
        int Line { get; set; }

        HashSet<INamedNode> FindNames(string name);
        HashSet<T> FindTypes<T>();
        int GetHashCode();
    }
}
