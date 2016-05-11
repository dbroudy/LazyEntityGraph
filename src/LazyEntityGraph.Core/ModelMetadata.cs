using System;
using System.Collections.Generic;
using System.Linq;
using LazyEntityGraph.Core.Constraints;

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
}