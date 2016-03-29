using Ploeh.AutoFixture.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LazyEntityGraph.AutoFixture
{
    public class MatchingTypeRequestSpecification : IRequestSpecification
    {
        private readonly IReadOnlyCollection<Type> _types;

        public MatchingTypeRequestSpecification(IReadOnlyCollection<Type> types)
        {
            _types = types;
        }

        public bool IsSatisfiedBy(object request)
        {
            var type = request as Type;
            return type != null && _types.Contains(type);
        }
    }
}
