using Ploeh.AutoFixture.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LazyEntityGraph.AutoFixture
{
    public class EntityPropertyCollectionRequestSpecification : IRequestSpecification
    {
        private readonly IReadOnlyCollection<Type> _entityTypes;

        public EntityPropertyCollectionRequestSpecification(IReadOnlyCollection<Type> entityTypes)
        {
            _entityTypes = entityTypes;
        }

        public bool IsSatisfiedBy(object request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var pi = request as PropertyInfo;
            if (pi == null)
                return false;

            if (!pi.GetGetMethod().IsVirtual)
                return false;

            if (!_entityTypes.Contains(pi.DeclaringType) && !_entityTypes.Contains(pi.DeclaringType.BaseType))
                return false;

            if (_entityTypes.Contains(pi.PropertyType))
                return true;

            var t = pi.PropertyType;
            return t.IsGenericType
                && t.GetGenericTypeDefinition() == typeof(ICollection<>)
                && _entityTypes.Contains(t.GenericTypeArguments[0]);
        }
    }
}
