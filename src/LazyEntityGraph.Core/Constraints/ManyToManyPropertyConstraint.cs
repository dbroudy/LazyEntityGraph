using LazyEntityGraph.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace LazyEntityGraph.Core.Constraints
{
    public class ManyToManyPropertyConstraint<THost, TProperty> : IPropertyConstraint<THost, ICollection<TProperty>>
        where THost : class
        where TProperty : class
    {
        private readonly PropertyInfo _inverse;

        public ManyToManyPropertyConstraint(
            Expression<Func<THost, ICollection<TProperty>>> propExpr,
            Expression<Func<TProperty, ICollection<THost>>> inverseExpr)
            : this(propExpr.GetProperty(), inverseExpr.GetProperty())
        {
        }

        public ManyToManyPropertyConstraint(PropertyInfo propInfo, PropertyInfo inverse)
        {
            PropInfo = propInfo;
            _inverse = inverse;
        }

        public void Rebind(THost host, ICollection<TProperty> previousValue, ICollection<TProperty> value)
        {
            foreach (var x in value)
            {
                CollectionProperty.Add(x, _inverse, host);
            }
        }

        public PropertyInfo PropInfo { get; }

        protected bool Equals(ManyToManyPropertyConstraint<THost, TProperty> other)
        {
            return Equals(_inverse, other._inverse) && Equals(PropInfo, other.PropInfo);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((ManyToManyPropertyConstraint<THost, TProperty>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_inverse != null ? _inverse.GetHashCode() : 0) * 397) ^
                       (PropInfo != null ? PropInfo.GetHashCode() : 0);
            }
        }
    }
}