using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LazyEntityGraph.Core.Extensions
{
    public class PropertyComparer : IEqualityComparer<PropertyInfo>
    {
        public bool Equals(PropertyInfo x, PropertyInfo y)
        {
            return x.PropertyEquals(y);
        }

        public int GetHashCode(PropertyInfo obj)
        {
            return obj.GetHashCode();
        }
    }
}
