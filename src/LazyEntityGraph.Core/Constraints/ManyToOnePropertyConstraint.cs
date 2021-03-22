using LazyEntityGraph.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace LazyEntityGraph.Core.Constraints
{
    public class ManyToOnePropertyConstraint<THost, TProperty> : IPropertyConstraint<THost, TProperty>
        where THost : class
        where TProperty : class
    {
        private readonly PropertyInfo _inverse;

        public ManyToOnePropertyConstraint(PropertyInfo propInfo, PropertyInfo inverse)
        {
            PropInfo = propInfo;
            _inverse = inverse;
        }

        public void Rebind(THost host, TProperty previousValue, TProperty value)
        {
            CollectionProperty.Remove(previousValue, _inverse, host);
            CollectionProperty.Add(value, _inverse, host);
        }

        public PropertyInfo PropInfo { get; }

        #region Equality
        protected bool Equals(ManyToOnePropertyConstraint<THost, TProperty> other)
        {
            return _inverse.PropertyEquals(other._inverse) && PropInfo.PropertyEquals(other.PropInfo);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((ManyToOnePropertyConstraint<THost, TProperty>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_inverse != null ? _inverse.GetHashCode() : 0) * 397) ^
                       (PropInfo != null ? PropInfo.GetHashCode() : 0);
            }
        }
        #endregion

        public override string ToString()
        {
            return $"{typeof(THost).Name}.{PropInfo.Name} *..1 {typeof(TProperty).Name}.{_inverse.Name}";
        }
    }
}