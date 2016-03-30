using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LazyEntityGraph.Core
{
    public static class Property
    {
        public static void Set<T, TValue>(T obj, PropertyInfo pi, TValue value)
            where TValue : class
        {
            var propertyAccessor = value as IPropertyAccessor<T>;
            if (propertyAccessor == null)
            {
                // property object is not proxy but we can still set the value
                pi.SetValue(obj, value);
                return;
            }

            var property = propertyAccessor.Get<TValue>(pi);
            TValue existing;
            if (property.TryGet(out existing) && existing == value)
                return;

            property.Set(value);
        }
    }

    public class Property<THost, TProperty> : IProperty<THost, TProperty>
        where TProperty : class
    {
        private readonly IEnumerable<IPropertyConstraint<THost, TProperty>> _constraints;
        private readonly THost _host;
        private readonly IInstanceCreator _instanceCreator;
        private bool _generate = true;

        private TProperty _value;

        public Property(THost host, PropertyInfo propInfo, IInstanceCreator instanceCreator,
            IEnumerable<IPropertyConstraint> constraints)
        {
            PropInfo = propInfo;
            _host = host;
            _instanceCreator = instanceCreator;
            _constraints = constraints.Cast<IPropertyConstraint<THost, TProperty>>();
        }

        public PropertyInfo PropInfo { get; }
        public bool IsSet => !_generate;

        public void Set(TProperty value)
        {
            _generate = false;
            _value = value;

            foreach (var c in _constraints)
                c.Bind(_host, _value);
        }

        public TProperty Get()
        {
            if (!_generate)
                return _value;

            Set(_instanceCreator.Create<TProperty>());

            return _value;
        }

        public bool TryGet(out TProperty value)
        {
            value = _value;
            return IsSet;
        }

        void IProperty<THost>.Set(object value)
        {
            Set((TProperty)value);
        }

        object IProperty<THost>.Get()
        {
            return Get();
        }
    }
}