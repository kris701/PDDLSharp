using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Search
{
    public class RefPriorityQueue
    {
        public HashSet<StateMove> ReferenceList { get; }
        public PriorityQueue<StateMove, int> Queue { get; }

        public RefPriorityQueue()
        {
            ReferenceList = new HashSet<StateMove>();
            Queue = new PriorityQueue<StateMove, int>();
        }

        public bool Contains(StateMove move) => ReferenceList.Contains(move);
        public int Count => Queue.Count;

        public void Enqueue(StateMove move, int priority)
        {
            Queue.Enqueue(move, priority);
            ReferenceList.Add(move);
        }

        public StateMove Dequeue()
        {
            var item = Queue.Dequeue();
            ReferenceList.Remove(item);
            return item;
        }
    }
}
