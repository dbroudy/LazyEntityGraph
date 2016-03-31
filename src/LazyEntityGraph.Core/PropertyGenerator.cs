using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LazyEntityGraph.Core
{
    class PropertyGenerator<T>
    {
        private readonly IReadOnlyCollection<Type> _entityTypes;
        private readonly IInstanceCreator _instanceCreator;
        private readonly IEnumerable<IPropertyConstraint> _constraints;

        private static bool IsCollection(Type t) => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ICollection<>);

        public PropertyGenerator(IReadOnlyCollection<Type> entityTypes, IInstanceCreator instanceCreator, IEnumerable<IPropertyConstraint> constraints)
        {
            _entityTypes = entityTypes;
            _instanceCreator = instanceCreator;
            _constraints = constraints;
        }

        public IEnumerable<IProperty<T>> Get(T host)
        {
            return typeof(T)
                .GetProperties()
                .Where(pi => pi.GetGetMethod().IsVirtual)
                .Where(pi => _entityTypes.Contains(pi.PropertyType) || IsCollection(pi.PropertyType))
                .Select(pi => GetProperty(pi, host));
        }

        private IProperty<T> GetProperty(PropertyInfo pi, T host)
        {
            var t = IsCollection(pi.PropertyType)
                ? typeof(CollectionProperty<,>).MakeGenericType(typeof(T), pi.PropertyType.GetGenericArguments()[0])
                : typeof(Property<,>).MakeGenericType(typeof(T), pi.PropertyType);

            var constraints = _constraints.Where(c => c.PropInfo == pi);

            return (IProperty<T>)Activator.CreateInstance(t, host, pi, _instanceCreator, constraints);
        }
    }
}