using LazyEntityGraph.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace LazyEntityGraph.Core
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
    }
}