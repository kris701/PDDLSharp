namespace PDDLSharp.Tools
{
    public static class QueueHelper
    {
        public static void EnqueueRange<T>(this Queue<T> set, Queue<T> other)
        {
            while(other.Count > 0)
                set.Enqueue(other.Dequeue());
        }
    }
}
