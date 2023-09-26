using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDL.Models
{
    public interface INode
    {
        public INode? Parent { get; }
        public int Start { get; set; }
        public int End { get; set; }
        public int Line { get; set; }

        public HashSet<INamedNode> FindNames(string name);
        public HashSet<T> FindTypes<T>();
        public int GetHashCode();
    }
}
