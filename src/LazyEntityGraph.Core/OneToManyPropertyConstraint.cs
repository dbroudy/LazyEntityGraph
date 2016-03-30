using LazyEntityGraph.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace LazyEntityGraph.Core
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

            collection.ItemAdded += x =>
            {
                Property.Set(x, _inverse, host);
            };

            foreach (var item in collection)
            {
                Property.Set(item, _inverse, host);
            }
        }
    }
}