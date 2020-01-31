using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LazyEntityGraph.Core.Constraints;

namespace LazyEntityGraph.Core
{
    public class Property<THost, TProperty> : IProperty<TProperty>
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

        public void Set(TProperty value)
        {
            var previousValue = _value;
            _generate = false;
            _value = value;

            foreach (var c in _constraints)
                c.Rebind(_host, previousValue, _value);
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
            return !_generate;
        }

        void IProperty.Set(object value)
        {
            Set((TProperty)value);
        }

        object IProperty.Get()
        {
            return Get();
        }
    }
}
