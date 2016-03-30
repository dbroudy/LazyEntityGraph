using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LazyEntityGraph.Core
{
    public class CollectionProperty<T, TProperty> : IProperty<T, ICollection<TProperty>>
        where TProperty : class
    {
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
                    foreach (var c in _constraints)
                        c.Bind(_host, _collection);
                }
                else
                {
                    _collection.Clear();
                    foreach (var x in value)
                        _collection.Add(x);
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
            return IsSet;
        }

        public PropertyInfo PropInfo { get; }

        public bool IsSet
        {
            get { return _collection != null; }
        }

        void IProperty<T>.Set(object value)
        {
            Set((ICollection<TProperty>)value);
        }

        object IProperty<T>.Get()
        {
            return Get();
        }
    }
}