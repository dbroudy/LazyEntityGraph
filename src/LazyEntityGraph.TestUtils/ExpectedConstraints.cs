using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LazyEntityGraph.Core.Constraints;

namespace LazyEntityGraph.TestUtils
{
    public class ExpectedConstraints
    {
        public static OneToOnePropertyConstraint<THost, TProperty> CreateOneToOne<THost, TProperty>(
                Expression<Func<THost, TProperty>> propExpr,
                Expression<Func<TProperty, THost>> inverseExpr)
            where THost : class
            where TProperty : class
        {
            return new OneToOnePropertyConstraint<THost, TProperty>(GetProperty(propExpr), GetProperty(inverseExpr));
        }

        public static OneToManyPropertyConstraint<THost, TProperty> CreateOneToMany<THost, TProperty>(
                Expression<Func<THost, ICollection<TProperty>>> propExpr,
                Expression<Func<TProperty, THost>> inverseExpr)
            where THost : class
            where TProperty : class
        {
            return new OneToManyPropertyConstraint<THost, TProperty>(GetProperty(propExpr), GetProperty(inverseExpr));
        }

        public static ManyToOnePropertyConstraint<THost, TProperty> CreateManyToOne<THost, TProperty>(
                Expression<Func<THost, TProperty>> propExpr,
                Expression<Func<TProperty, ICollection<THost>>> inverseExpr)
            where THost : class
            where TProperty : class
        {
            return new ManyToOnePropertyConstraint<THost, TProperty>(GetProperty(propExpr), GetProperty(inverseExpr));
        }

        public static ManyToManyPropertyConstraint<THost, TProperty> CreateManyToMany<THost, TProperty>(
                Expression<Func<THost, ICollection<TProperty>>> propExpr,
                Expression<Func<TProperty, ICollection<THost>>> inverseExpr)
            where THost : class
            where TProperty : class
        {
            return new ManyToManyPropertyConstraint<THost, TProperty>(GetProperty(propExpr), GetProperty(inverseExpr));
        }


        public static ForeignKeyConstraint<T, TProp, TKey> CreateForeignKey<T, TProp, TKey>(
                Expression<Func<T, TProp>> navProp,
                Expression<Func<T, TKey>> foreignKeyProp,
                Expression<Func<TProp, TKey>> idProp)
            where T : class
            where TProp : class
        {
            return new ForeignKeyConstraint<T, TProp, TKey>(GetProperty(navProp), GetProperty(foreignKeyProp), GetProperty(idProp));
        }

        public static PropertyInfo GetProperty<T, TProp>(Expression<Func<T, TProp>> expr)
        {
            return (PropertyInfo)((MemberExpression)expr.Body).Member;
        }

    }
}
