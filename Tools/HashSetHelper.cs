using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
