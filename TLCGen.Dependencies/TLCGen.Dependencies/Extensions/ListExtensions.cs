using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Extensions
{
    public static class ListExtensions
    {
        public static void BubbleSort(this IList o)
        {
            for (var i = o.Count - 1; i >= 0; i--)
            {
                for (var j = 1; j <= i; j++)
                {
                    var o1 = o[j - 1];
                    var o2 = o[j];
                    if (((IComparable)o1).CompareTo(o2) > 0)
                    {
                        o.Remove(o1);
                        o.Insert(j, o1);
                    }
                }
            }
        }

        public static void BubbleSort<T>(this IList o, Func<T, T, int> compareFunc)
        {
            for (var i = o.Count - 1; i >= 0; i--)
            {
                for (var j = 1; j <= i; j++)
                {
                    var o1 = (T)o[j - 1];
                    var o2 = (T)o[j];
                    if (compareFunc(o1, o2) > 0)
                    {
                        o.Remove(o1);
                        o.Insert(j, o1);
                    }
                }
            }
        }

        public static bool IsSorted(this IList o)
        {
            for (var i = o.Count - 1; i >= 0; i--)
            {
                for (var j = 1; j <= i; j++)
                {
                    var o1 = o[j - 1];
                    var o2 = o[j];
                    if (((IComparable)o1).CompareTo(o2) > 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static void RemoveAll(this IList list)
        {
            while (list.Count > 0)
            {
                list.RemoveAt(list.Count - 1);
            }
        }

        public static void RemoveSome<TSource>(this IList<TSource> source, Func<TSource, bool> predicate)
        {
            var remElems = new List<TSource>();
            foreach(var e in source)
            {
                if (predicate(e)) remElems.Add(e);
            }
            foreach(var e in remElems)
            {
                source.Remove(e);
            }
        }
    }
}
