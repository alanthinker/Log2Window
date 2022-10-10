using ICSharpCode.TextEditor.Actions;
using Log2Window.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log2Window
{
    public class MyCategoryList<T, CAT>
    {
        Dictionary<CAT, MyList<T>> _categories = new Dictionary<CAT, MyList<T>>();

        public MyCategoryList(List<CAT> cats)
        {
            foreach (var cat in cats)
            {
                if (!_categories.ContainsKey(cat))
                {
                    _categories.Add(cat, new MyList<T>());
                }
            }
        }

        public long AllItemsCount
        {
            get
            {
                long count = 0;
                foreach (var pair in _categories)
                {
                    count += pair.Value.Count;
                }
                return count;
            }
        }

        public void Enqueue(T item, CAT cat)
        {
            _categories[cat].Enqueue(item);
        }

        public void Clear()
        {
            foreach (var pair in _categories)
            {
                pair.Value.Clear();
            }
        }

        // 为了防止反复多次调用 DequeueSmart, 只要在每个cat的元素数量超过 110%的 catMaxItemsCount 的时候, 才执行 Dequeue,
        // 一旦执行, 最终元素数量被减小到 100% 以内.
        public long DequeueSmart(long catMaxItemsCount)
        {
            var max_110_percent = catMaxItemsCount * 1.1;

            long dequeuedCount = 0;
            foreach (var pair in _categories)
            {
                if (pair.Value.Count > max_110_percent)
                {
                    while (pair.Value.Count > catMaxItemsCount)
                    {
                        pair.Value.Dequeue();
                        dequeuedCount++;
                    }
                }
            }

            return dequeuedCount;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var cat in _categories)
            {
                foreach (var item in cat.Value)
                {
                    yield return item;
                }
            }
        }

        public List<MyList<T>> ToListList()
        {
            List<MyList<T>> list = new List<MyList<T>>();
            foreach (var cat in _categories)
            {
                list.Add(cat.Value);
            }
            return list;
        }
    }   
}
