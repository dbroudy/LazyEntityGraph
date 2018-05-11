using LazyEntityGraph.Core.Constraints;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LazyEntityGraph.Core
{
    public static class CollectionProperty
    {
        public static void Add<T, TValue>(T obj, PropertyInfo pi, TValue value)
            where T : class
            where TValue : class
        {
            if (!(obj is IPropertyAccessor<T> propertyAccessor))
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
            if (!(obj is IPropertyAccessor<T> propertyAccessor))
            {
                var collection = pi.GetValue(obj) as ICollection<TValue>;
                collection?.Remove(value);
                return;
            }

            var collectionProperty = (ICollectionProperty<T, TValue>)propertyAccessor.Get<ICollection<TValue>>(pi);
            collectionProperty.Remove(value);
        }
    }

    public interface ICollectionProperty<T, TProperty> : IProperty<T, ICollection<TProperty>>
    {
        void Insert(TProperty item);
        void Remove(TProperty item);

        event CollectionEventHandler<TProperty> ItemAdded;
    }

    public class CollectionProperty<T, TProperty> : ICollectionProperty<T, TProperty>
        where TProperty : class
    {
        private readonly HashSet<TProperty> _addOnCreation = new HashSet<TProperty>();
        private readonly IEnumerable<IPropertyConstraint<T, ICollection<TProperty>>> _constraints;
        private readonly T _host;
        private readonly IInstanceCreator _instanceCreator;
        private LazyEntityCollection<TProperty> _collection;

        public CollectionProperty(T host, PropertyInfo propInfo, IInstanceCreator instanceCreator,
            IEnumerable<IPropertyConstraint> constraints)
        {
            _host = host;
            _instanceCreator = instanceCreator;
            PropInfo = propInfo;
            _constraints = constraints.Cast<IPropertyConstraint<T, ICollection<TProperty>>>();
        }

        public void Set(ICollection<TProperty> value)
        {
            if (value == null)
            {
                _collection?.Clear();
            }
            else
            {
                if (_collection == null)
                {
                    _collection = new LazyEntityCollection<TProperty>(value);
                    _collection.ItemAdded += ItemAdded;
                    foreach (var item in _addOnCreation)
                        _collection.Add(item);
                    foreach (var c in _constraints)
                        c.Rebind(_host, null, _collection);
                }
                else
                {
                    var add = value.Except(_collection).ToList();
                    var remove = _collection.Except(value).ToList();
                    foreach (var item in add)
                        _collection.Add(item);
                    foreach (var item in remove)
                        _collection.Remove(item);
                }
            }
        }

        public ICollection<TProperty> Get()
        {
            if (_collection != null)
                return _collection;

            var generated = _instanceCreator.Create<ICollection<TProperty>>();
            Set(generated);
            return _collection;
        }

        public bool TryGet(out ICollection<TProperty> value)
        {
            value = _collection;
            return _collection != null;
        }

        public PropertyInfo PropInfo { get; }

        void IProperty<T>.Set(object value)
        {
            Set((ICollection<TProperty>)value);
        }

        object IProperty<T>.Get()
        {
            return Get();
        }

        public void Insert(TProperty item)
        {
            if (_collection != null && !_collection.Contains(item))
            {
                _collection.Add(item);
            }
            else
            {
                _addOnCreation.Add(item);
            }
        }

        public void Remove(TProperty item)
        {
            if (_collection != null)
            {
                _collection.Remove(item);
            }
            else
            {
                _addOnCreation.Remove(item);
            }
        }

        public event CollectionEventHandler<TProperty> ItemAdded;
    }
}