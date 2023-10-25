using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Toolkit.Grounders
{
    public interface IGrounder<T>
    {
        public List<T> Ground(T item);
        public Queue<int[]> GenerateParameterPermutations(List<NameExp> parameters);

        public int GetIndexFromObject(string obj);
        public string GetObjectFromIndex(int index);
        public int GetIndexFromType(string type);
        public string GetTypeFromIndex(int index);
    }
}
