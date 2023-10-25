using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Toolkit.Grounders
{
    public interface IGrounder<T>
    {
        public List<T> Ground(T item);
        public List<int[]> GenerateParameterPermutations(List<NameExp> parameters);

        public int GetIndexFromObject(NameExp obj);
        public NameExp GetObjectFromIndex(int index);
        public int GetIndexFromType(TypeExp type);
        public TypeExp GetTypeFromIndex(int index);
    }
}
