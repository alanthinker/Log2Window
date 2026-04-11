using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Log2Window
{
    /// <summary>
    /// A collection which can support index, Enqueue, Dequeue, Peek, GetEnumerator with very good performance.
    /// Have both advantage of Queue and List.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MyList<T>
    {
        private List<T> list = new List<T>();
        private int startIndex = 0;
        private int endIndexAddOne = 0;//not include;

        private int resetCount = 1000;
        public long StateId { get; private set; }

        public T this[int index]
        {
            get
            {
                return list[index + startIndex];
            }
        }

        public int Count
        {
            get
            {
                return endIndexAddOne - startIndex;
            }
        }

        public void Enqueue(T item)
        {
            StateId++;
            list.Add(item);
            endIndexAddOne++;

            if (startIndex > resetCount && startIndex > endIndexAddOne / 2)
            {
                var newlist = new List<T>(this.Count);
                foreach (var temp in this)
                {
                    newlist.Add(temp);
                }
                list.Clear();
                list = newlist;
                startIndex = 0;
                endIndexAddOne = list.Count;
            }
        }

        public void Clear()
        {
            StateId++;
            list.Clear();
            endIndexAddOne = 0;
            startIndex = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = startIndex; i < endIndexAddOne; i++)
            {
                yield return list[i];
            }
        }

        public T Peek()
        {
            return list[startIndex];
        }

        public T Dequeue()
        {
            StateId++;
            var item = this.Peek();
            startIndex++;
            return item;
        }

        public void Sort()
        {
            this.list.Sort(startIndex, endIndexAddOne - startIndex, Comparer<T>.Default);
        }

        /// <summary>
        /// Compact the internal list by removing dequeued items and freeing memory.
        /// </summary>
        public void TrimExcess()
        {
            if (startIndex > 0)
            {
                StateId++;
                var newlist = new List<T>(this.Count);
                for (int i = startIndex; i < endIndexAddOne; i++)
                {
                    newlist.Add(list[i]);
                }
                list.Clear();
                list = newlist;
                startIndex = 0;
                endIndexAddOne = list.Count;
            }
        }

        /// <summary>
        /// Remove specified number of items from head. O(1) operation.
        /// </summary>
        /// <param name="count">Number of items to remove.</param>
        public void RemoveRangeFromHead(int count)
        {
            if (count <= 0) return;
            if (count > this.Count) count = this.Count;
            
            StateId++;
            startIndex += count;
        }

        /// <summary>
        /// Find the index of first item where predicate returns false using binary search.
        /// Assumes list is sorted by the property being compared.
        /// </summary>
        /// <param name="predicate">Return true for items to be removed (smaller items).</param>
        /// <returns>Index of first item to keep, or Count if all items should be removed.</returns>
        public int FindFirstIndexToKeep(Func<T, bool> predicate)
        {
            // Binary search: find first index where predicate returns false
            int left = 0;
            int right = this.Count;
            
            while (left < right)
            {
                int mid = left + (right - left) / 2;
                if (predicate(this[mid]))
                {
                    // Item at mid should be removed, search in right half
                    left = mid + 1;
                }
                else
                {
                    // Item at mid should be kept, search in left half (including mid)
                    right = mid;
                }
            }
            
            return left; // First index to keep
        }
    }
}
