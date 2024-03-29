﻿namespace PDDLSharp.Toolkit.Planners.Tools
{
    public class RefPriorityQueue
    {
        public HashSet<int> ReferenceList { get; }
        public PriorityQueue<StateMove, int> Queue { get; }

        public RefPriorityQueue()
        {
            ReferenceList = new HashSet<int>();
            Queue = new PriorityQueue<StateMove, int>();
        }

        public bool Contains(StateMove move) => ReferenceList.Contains(move.GetHashCode());
        public int Count => Queue.Count;
        public void Clear()
        {
            ReferenceList.Clear();
            Queue.Clear();
        }

        public void Enqueue(StateMove move, int priority)
        {
            Queue.Enqueue(move, priority);
            ReferenceList.Add(move.GetHashCode());
        }

        public StateMove Dequeue()
        {
            var item = Queue.Dequeue();
            ReferenceList.Remove(item.GetHashCode());
            return item;
        }

        public List<StateMove> DequeueAll()
        {
            var list = new List<StateMove>();
            while (Queue.Count > 0)
                list.Add(Dequeue());
            return list;
        }
    }
}
