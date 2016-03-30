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

        public void Bind(THost host, ICollection<TProperty> value)
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

    public class ManyToOnePropertyConstraint<THost, TProperty> : IPropertyConstraint<THost, TProperty>
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

        public void Bind(THost host, TProperty value)
        {
            // host = foo
            // value = bar
            // inverse = bar.Foos


            throw new NotImplementedException();
        }

        public PropertyInfo PropInfo { get; }
    }
}