using System.Collections.Generic;
using System.Reflection;

namespace LazyEntityGraph.Core
{
    public static class CollectionPropertyHelper
    {
        public static void Add<T, TValue>(T obj, PropertyInfo pi, TValue value)
            where T : class
            where TValue : class
        {
            var propertyAccessor = obj as IPropertyAccessor<T>;
            if (propertyAccessor == null)
            {
                var collection = pi.GetValue(obj) as ICollection<TValue>;
                collection?.Add(value);
                return;
            }

            var collectionProperty = (ICollectionProperty<T, TValue>)propertyAccessor.Get<ICollection<TValue>>(pi);
            collectionProperty.Insert(value);
        }

        public static void Remove<T, TValue>(T obj, PropertyInfo pi, TValue value)
            where T : class
            where TValue : class
        {
            if (obj == null)
                return;
            var propertyAccessor = obj as IPropertyAccessor<T>;
            if (propertyAccessor == null)
            {
                var collection = pi.GetValue(obj) as ICollection<TValue>;
                collection?.Remove(value);
                return;
            }

            var collectionProperty = (ICollectionProperty<T, TValue>)propertyAccessor.Get<ICollection<TValue>>(pi);
            collectionProperty.Remove(value);
        }
    }
}
