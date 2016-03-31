using LazyEntityGraph.Core.Extensions;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace LazyEntityGraph.Core.Constraints
{
    public class OneToOnePropertyConstraint<THost, TProperty> : IPropertyConstraint<THost, TProperty>
        where THost : class
        where TProperty : class
    {
        private readonly PropertyInfo _inverse;

        public OneToOnePropertyConstraint(
            Expression<Func<THost, TProperty>> propExpr,
            Expression<Func<TProperty, THost>> inverseExpr)
            : this(propExpr.GetProperty(), inverseExpr.GetProperty())
        {
        }

        public OneToOnePropertyConstraint(PropertyInfo propInfo, PropertyInfo inverse)
        {
            PropInfo = propInfo;
            _inverse = inverse;
        }

        public void Rebind(THost host, TProperty previousValue, TProperty value)
        {
            Property.Set(value, _inverse, host);
        }

        public PropertyInfo PropInfo { get; }

        protected bool Equals(OneToOnePropertyConstraint<THost, TProperty> other)
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
            return Equals((OneToOnePropertyConstraint<THost, TProperty>)obj);
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