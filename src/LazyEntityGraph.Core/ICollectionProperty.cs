using System.Collections.Generic;

namespace LazyEntityGraph.Core
{
    public interface ICollectionProperty<out T, TProperty> : IProperty<ICollection<TProperty>>
    {
        void Insert(TProperty item);
        void Remove(TProperty item);

        event CollectionEventHandler<TProperty> ItemAdded;
    }
}
