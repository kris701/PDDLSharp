using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Toolkit.Grounders
{
    public interface IGrounder<T>
    {
        public List<T> Ground(T item);
        public List<List<string>> GenerateParameterPermutations(List<NameExp> parameters);
    }
}
