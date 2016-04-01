using LazyEntityGraph.Core;
using LazyEntityGraph.Core.Constraints;
using Ploeh.AutoFixture.Kernel;
using System;
using System.Collections.Generic;

namespace LazyEntityGraph.AutoFixture
{
    public class EntitySpecimenBuilder : ISpecimenBuilder
    {
        private readonly IReadOnlyCollection<Type> _entityTypes;
        private readonly IReadOnlyCollection<IPropertyConstraint> _constraints;

        public EntitySpecimenBuilder(IReadOnlyCollection<Type> entityTypes, IReadOnlyCollection<IPropertyConstraint> constraints)
        {
            if (entityTypes == null)
                throw new ArgumentNullException(nameof(entityTypes));
            if (constraints == null)
                throw new ArgumentNullException(nameof(constraints));

            _entityTypes = entityTypes;
            _constraints = constraints;
        }

        class SpecimenContextInstanceCreatorAdapter : IInstanceCreator
        {
            private readonly ISpecimenContext _context;

            public SpecimenContextInstanceCreatorAdapter(ISpecimenContext context)
            {
                _context = context;
            }

            public object Create(Type type)
            {
                return _context.Resolve(type);
            }
        }

        public object Create(object request, ISpecimenContext context)
        {
            var t = request as Type;
            if (t == null)
                return context.Resolve(request);

            var adapter = new SpecimenContextInstanceCreatorAdapter(context);
            var proxyCreator = new ProxyInstanceCreator(adapter, _entityTypes, _constraints);
            return proxyCreator.Create(t);
        }
    }
}
