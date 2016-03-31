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

        public ManyToOnePropertyConstraint(
            Expression<Func<THost, TProperty>> propExpr,
            Expression<Func<TProperty, ICollection<THost>>> inverseExpr)
            : this(propExpr.GetProperty(), inverseExpr.GetProperty())
        {
        }

        public ManyToOnePropertyConstraint(PropertyInfo propInfo, PropertyInfo inverse)
        {
            PropInfo = propInfo;
            _inverse = inverse;
        }

        public void Rebind(THost host, TProperty previousValue, TProperty value)
        {
            CollectionProperty.Add(value, _inverse, host);
        }

        public PropertyInfo PropInfo { get; }
    }
}