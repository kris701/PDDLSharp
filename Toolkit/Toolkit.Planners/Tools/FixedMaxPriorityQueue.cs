using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Tools
{
    /// <summary>
    /// In a max priority queue, elements are inserted in the order in which they arrive the queue and the maximum value is always removed first from the queue. For example, assume that we insert in the order 8, 3, 2 & 5 and they are removed in the order 8, 5, 3, 2.
    /// When the queue is full, the largest element is removed first.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FixedMaxPriorityQueue<T>
    {
        public int Count => _keys.Count;
        public int Size { get; }

        private List<int> _keys;
        private List<T> _values;

        public FixedMaxPriorityQueue(int size)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException("Size must be larger than 0!");
            Size = size;
            _keys = new List<int>(size);
            _values = new List<T>(size);
        }

        public void Enqueue(T value, int key)
        {
            if (Count >= Size)
            {
                _keys.RemoveAt(0);
                _values.RemoveAt(0);
            }

            int insertIndex = GetInsertIndex(key);
            _keys.Insert(insertIndex, key);
            _values.Insert(insertIndex, value);
        }

        private int GetInsertIndex(int key)
        {
            for (int i = 0; i < Count; i++)
                if (key > _keys[i])
                    return i;
            return 0;
        }

        public T Dequeue()
        {
            if (_keys.Count <= 0)
                throw new ArgumentException("The queue is already empty!");
            var returnValue = _values[0];
            _values.RemoveAt(0);
            _keys.RemoveAt(0);
            return returnValue;
        }
    }
}
