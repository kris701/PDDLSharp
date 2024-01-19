using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Tools
{
    public class FixedPriorityQueue<T>
    {
        public int Count => _keys.Count;

        private List<int> _keys;
        private List<T> _values;
        private int _size = 0;

        public FixedPriorityQueue(int size)
        {
            _size = size;
            _keys = new List<int>(size);
            _values = new List<T>(size);
        }

        public void Enqueue(T value, int key)
        {
            if (Count >= _size)
            {
                if (key > _keys[_size - 1])
                    return;
                else
                {
                    _keys.RemoveAt(_size - 1);
                    _values.RemoveAt(_size - 1);
                }
            }

            int insertIndex = 0;
            for (int i = 0; i < _keys.Count; i++)
            {
                if (key < _keys[i])
                {
                    insertIndex = i;
                    break;
                }
            }
            _keys.Insert(insertIndex, key);
            _values.Insert(insertIndex, value);
        }

        public T Dequeue()
        {
            var returnValue = _values[0];
            _values.RemoveAt(0);
            _keys.RemoveAt(0);
            return returnValue;
        }
    }
}
