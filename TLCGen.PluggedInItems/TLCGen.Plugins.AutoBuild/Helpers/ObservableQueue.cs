using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace TLCGen.Plugins.AutoBuild
{
    public class ObservableQueue<T> : INotifyCollectionChanged, IEnumerable<T>
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        private readonly Queue<T> queue = new Queue<T>();

        public int Count
        {
            get { return queue.Count; }
        }

        public T this[int index]    // Indexer declaration
        {
            get
            {
                return queue.ElementAt(index);
            }
        }

        public void Enqueue(T item)
        {
            queue.Enqueue(item);

            if (CollectionChanged != null)
                CollectionChanged(this,
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Add, item));
            if (queue.Count >= 250)
            {
                Dequeue();
            }
        }

        public T Dequeue()
        {
            var item = queue.Dequeue();
            if (CollectionChanged != null)
                CollectionChanged(this,
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Remove, item, 0));
            return item;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
