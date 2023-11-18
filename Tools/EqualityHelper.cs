using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PDDLSharp.Tools
{
    public static class EqualityHelper
    {
        public static bool AreListsEqual<T>(List<T> list1, List<T> list2)
        {
            if (list1 == null && list2 == null) return true;
            if (list1 == null && list2 != null) return false;
            if (list1 != null && list2 == null) return false;
            if (list1 != null && list2 != null)
            {
                if (list1.Count != list2.Count) return false;
                for (int i = 0; i < list1.Count; i++)
                {
                    if (list1[i] == null && list2[i] == null) continue;
                    if (list1[i] == null && list2[i] != null) return false;
                    if (list1[i] != null && list2[i] == null) return false;
                    if (list1[i] != null && list2[i] != null)
                        if (!list1[i].Equals(list2[i]))
                            return false;
                }
            }
            return true;
        }

        public static bool AreListsEqual<T>(T[] list1, T[] list2)
        {
            if (list1 == null && list2 == null) return true;
            if (list1 == null && list2 != null) return false;
            if (list1 != null && list2 == null) return false;
            if (list1 != null && list2 != null)
            {
                if (list1.Length != list2.Length) return false;
                for (int i = 0; i < list1.Length; i++)
                {
                    if (list1[i] == null && list2[i] == null) continue;
                    if (list1[i] == null && list2[i] != null) return false;
                    if (list1[i] != null && list2[i] == null) return false;
                    if (list1[i] != null && list2[i] != null)
                        if (!list1[i].Equals(list2[i]))
                            return false;
                }
            }
            return true;
        }

        public static bool AreListsEqualUnordered<T>(List<T> list1, List<T> list2)
        {
            if (list1 == null && list2 == null) return true;
            if (list1 == null && list2 != null) return false;
            if (list1 != null && list2 == null) return false;
            if (list1 != null && list2 != null)
            {
                if (list1.Count != list2.Count) return false;
                foreach(var item in list1)
                    if (!list2.Contains(item))
                        return false;
            }
            return true;
        }

        public static bool AreListsEqualUnordered<T>(T[] list1, T[] list2)
        {
            if (list1 == null && list2 == null) return true;
            if (list1 == null && list2 != null) return false;
            if (list1 != null && list2 == null) return false;
            if (list1 != null && list2 != null)
            {
                if (list1.Length != list2.Length) return false;
                foreach (var item in list1)
                    if (!list2.Contains(item))
                        return false;
            }
            return true;
        }

        public static bool AreListsEqual<T>(HashSet<T> list1, HashSet<T> list2)
        {
            if (list1 == null && list2 == null) return true;
            if (list1 == null && list2 != null) return false;
            if (list1 != null && list2 == null) return false;
            if (list1 != null && list2 != null)
            {
                if (list1.Count != list2.Count) return false;
                foreach (var item in list1)
                    if (!list2.Contains(item))
                        return false;
            }
            return true;
        }
    }
}
