using LazyEntityGraph.Core.Extensions;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace LazyEntityGraph.Core
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
    }
}