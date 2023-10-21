using PDDLSharp.Models.PDDL.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Grounders
{
    public interface IGrounder<T>
    {
        public List<T> Ground(T item);
        public List<List<string>> GenerateParameterPermutations(List<NameExp> parameters);
    }
}
