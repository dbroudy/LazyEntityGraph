using Castle.DynamicProxy;
using LazyEntityGraph.Core.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LazyEntityGraph.Core
{
    public class ProxyInstanceCreator : IInstanceCreator
    {
        private readonly IReadOnlyCollection<Type> _entityTypes;
        private readonly IInstanceCreator _instanceCreator;
        private readonly IReadOnlyCollection<IPropertyConstraint> _constraints;
        private readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();

        public ProxyInstanceCreator(IReadOnlyCollection<Type> entityTypes, IInstanceCreator instanceCreator, IReadOnlyCollection<IPropertyConstraint> constraints)
        {
            if (instanceCreator == null)
                throw new ArgumentNullException(nameof(instanceCreator));
            if (constraints == null)
                throw new ArgumentNullException(nameof(constraints));

            _entityTypes = entityTypes;
            _instanceCreator = instanceCreator;
            _constraints = constraints;
        }

        public object Create(Type type)
        {
            if (!_entityTypes.Contains(type))
                return _instanceCreator.Create(type);

            return GetType()
                .GetMethod("CreateGeneric", BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(type)
                .Invoke(this, new object[] { });
        }

        private T CreateGeneric<T>()
        {
            var interceptor = new VirtualPropertyInterceptor<T>();
            var entity = (T)_proxyGenerator.CreateClassProxy(typeof(T), new[] { typeof(IPropertyAccessor<T>) }, interceptor);
            var propertyGenerator = new PropertyGenerator<T>(_entityTypes, _instanceCreator, _constraints);
            interceptor.SetProperties(propertyGenerator.Get(entity));
            return entity;
        }
    }
}
