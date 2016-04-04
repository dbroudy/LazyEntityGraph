using Castle.DynamicProxy;
using LazyEntityGraph.Core.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LazyEntityGraph.Core
{
    public class ModelMetadata
    {
        public ModelMetadata(IEnumerable<Type> entityTypes, IEnumerable<IPropertyConstraint> constraints)
        {
            EntityTypes = entityTypes.Distinct().ToList();
            Constraints = constraints.Distinct().ToList();
        }

        public IReadOnlyCollection<Type> EntityTypes { get; }
        public IReadOnlyCollection<IPropertyConstraint> Constraints { get; }
    }

    public class ProxyInstanceCreator : IInstanceCreator
    {
        private readonly IReadOnlyCollection<IPropertyConstraint> _constraints;
        private readonly IReadOnlyCollection<Type> _entityTypes;
        private readonly IInstanceCreator _instanceCreator;
        private readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();

        public ProxyInstanceCreator(IInstanceCreator instanceCreator, ModelMetadata modelMetadata)
        {
            if (instanceCreator == null)
                throw new ArgumentNullException(nameof(instanceCreator));
            if (modelMetadata == null)
                throw new ArgumentNullException(nameof(modelMetadata));

            _entityTypes = modelMetadata.EntityTypes;
            _instanceCreator = instanceCreator;
            _constraints = modelMetadata.Constraints;
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
            var entity =
                (T)_proxyGenerator.CreateClassProxy(typeof(T), new[] { typeof(IPropertyAccessor<T>) }, interceptor);
            var propertyGenerator = new PropertyFactory<T>(_instanceCreator, _entityTypes, _constraints);
            interceptor.SetProperties(propertyGenerator.Get(entity));
            return entity;
        }
    }
}