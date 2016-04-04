using LazyEntityGraph.Core;
using LazyEntityGraph.Core.Constraints;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace LazyEntityGraph.Edmx
{
    public class EdmxProcessor
    {
        public ModelMetadata GetMetadata(string filePath)
        {
            var entityTypes = new HashSet<Type>();
            var constraints = new HashSet<IPropertyConstraint>();
            var xml = XDocument.Load(filePath);

            return new ModelMetadata(entityTypes, constraints);
        }
    }
}
