namespace PDDLSharp.Tools
{
    public static class HashSetHelper
    {
        public static void AddRange<T>(this HashSet<T> set, HashSet<T> other)
        {
            foreach (var item in other)
                set.Add(item);
        }
    }
}
