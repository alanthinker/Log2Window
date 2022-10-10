using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log2Window
{
    class NListsMerger<T> where T : IComparable<T>
    {
        private MyList<T> Merge2Lists(MyList<T> arr1, MyList<T> arr2)
        {
            int len1 = arr1.Count;
            int len2 = arr2.Count;

            // 定义结果数组
            var merged = new MyList<T>();

            // 定义索引位置
            int loc1, loc2;
            loc1 = loc2 = 0;

            // 遍历两个数组
            while (loc1 < len1 && loc2 < len2)
            {
                if (arr1[loc1].CompareTo(arr2[loc2]) < 0)
                {
                    merged.Enqueue(arr1[loc1++]);
                }
                else
                {
                    merged.Enqueue(arr2[loc2++]);
                }
            }

            // 拷贝较长数组余下的元素
            while (loc1 < len1)
            {
                merged.Enqueue(arr1[loc1++]);
            }

            while (loc2 < len2)
            {
                merged.Enqueue(arr2[loc2++]);
            }

            return merged;
        }

        public MyList<T> MergeNLists(List<MyList<T>> lists)
        {
            if (lists.Count == 0)
            {
                return null;
            }
            return Merge(lists, 0, lists.Count - 1);
        }

        private MyList<T> Merge(List<MyList<T>> lists, int lo, int hi)
        {
            if (lo == hi)
            {
                return lists[lo];
            }
            int mid = lo + (hi - lo) / 2;
            MyList<T> l1 = Merge(lists, lo, mid);
            MyList<T> l2 = Merge(lists, mid + 1, hi);
            return Merge2Lists(l1, l2);
        }
    }
}
