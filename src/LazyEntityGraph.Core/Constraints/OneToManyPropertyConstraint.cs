using LazyEntityGraph.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace LazyEntityGraph.Core.Constraints
{
    public class OneToManyPropertyConstraint<THost, TProperty> : IPropertyConstraint<THost, ICollection<TProperty>>
        where THost : class
        where TProperty : class
    {
        private readonly PropertyInfo _inverse;

        public OneToManyPropertyConstraint(
            Expression<Func<THost, ICollection<TProperty>>> propExpr,
            Expression<Func<TProperty, THost>> inverseExpr)
            : this(propExpr.GetProperty(), inverseExpr.GetProperty())
        {
        }

        protected bool Equals(OneToManyPropertyConstraint<THost, TProperty> other)
        {
            return Equals(_inverse, other._inverse) && Equals(PropInfo, other.PropInfo);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((OneToManyPropertyConstraint<THost, TProperty>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_inverse != null ? _inverse.GetHashCode() : 0) * 397) ^ (PropInfo != null ? PropInfo.GetHashCode() : 0);
            }
        }

        public OneToManyPropertyConstraint(PropertyInfo propInfo, PropertyInfo inverse)
        {
            PropInfo = propInfo;
            _inverse = inverse;
        }

        public PropertyInfo PropInfo { get; }

        public void Rebind(THost host, ICollection<TProperty> previousValue, ICollection<TProperty> value)
        {
            var collection = value as LazyEntityCollection<TProperty>;
            if (collection == null)
                return;

            collection.ItemAdded += x => Property.Set(x, _inverse, host);

            foreach (var item in value)
            {
                Property.Set(item, _inverse, host);
            }
        }
    }
}