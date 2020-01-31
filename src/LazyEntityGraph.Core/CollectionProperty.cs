using LazyEntityGraph.Core.Constraints;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LazyEntityGraph.Core
{
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
                if (_collection != null)
                {
                    _collection.Clear();
                }
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

        void IProperty.Set(object value)
        {
            Set((ICollection<TProperty>)value);
        }

        object IProperty.Get()
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
