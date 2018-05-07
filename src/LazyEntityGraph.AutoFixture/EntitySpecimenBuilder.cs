using LazyEntityGraph.Core;
using AutoFixture.Kernel;
using System;

namespace LazyEntityGraph.AutoFixture
{
    public class EntitySpecimenBuilder : ISpecimenBuilder
    {
        private readonly ModelMetadata _modelMetadata;

        public EntitySpecimenBuilder(ModelMetadata modelMetadata)
        {
            if (modelMetadata == null)
                throw new ArgumentNullException(nameof(modelMetadata));

            _modelMetadata = modelMetadata;
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
            var proxyCreator = new ProxyInstanceCreator(adapter, _modelMetadata);
            return proxyCreator.Create(t);
        }
    }
}
