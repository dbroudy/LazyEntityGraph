using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LazyEntityGraph.Core
{
    public class LazyEntityCollection<T> : Collection<T>
    {
        public LazyEntityCollection()
        {

        }

        public LazyEntityCollection(IEnumerable<T> collection)
            : base(collection.ToList())
        {
        }


        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            Raise(ItemAdded, item);
        }

        protected override void SetItem(int index, T item)
        {
            var existing = this[index];
            base.SetItem(index, item);
            Raise(ItemAdded, item);
            if (existing != null)
                Raise(ItemRemoved, existing);
        }

        protected override void RemoveItem(int index)
        {
            var item = this[index];
            base.RemoveItem(index);
            if (item != null)
                Raise(ItemRemoved, item);
        }

        protected override void ClearItems()
        {
            var items = this.ToList();
            base.ClearItems();
            foreach (var item in items)
                Raise(ItemRemoved, item);
        }

        public delegate void CollectionEventHandler(T item);

        public event CollectionEventHandler ItemAdded;
        public event CollectionEventHandler ItemRemoved;

        private static void Raise(CollectionEventHandler handler, T item) => handler?.Invoke(item);
    }
}
