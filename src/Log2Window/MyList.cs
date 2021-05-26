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
    }
}
