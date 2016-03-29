using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyEntityGraph.Core;
using Ploeh.AutoFixture.Kernel;

namespace LazyEntityGraph.AutoFixture
{
    public class EntitySpecimenBuilder : ISpecimenBuilder
    {
        class InstanceCreator : IInstanceCreator
        {
            private readonly ISpecimenContext _context;

            public InstanceCreator(ISpecimenContext context)
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

            var proxyCreator = new ProxyInstanceCreator(new InstanceCreator(context));
            return proxyCreator.Create(t);
        }
    }
}
