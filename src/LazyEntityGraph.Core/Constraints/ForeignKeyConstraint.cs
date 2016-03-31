using LazyEntityGraph.Core.Extensions;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace LazyEntityGraph.Core.Constraints
{
    public class ForeignKeyConstraint<T, TProp, TKey> : IPropertyConstraint<T, TProp>
        where T : class
        where TProp : class
    {
        private readonly PropertyInfo _foreignKeyProp;
        private readonly PropertyInfo _idProp;

        public ForeignKeyConstraint(Expression<Func<T, TProp>> navProp, Expression<Func<T, TKey>> foreignKeyProp, Expression<Func<TProp, TKey>> idProp)
            : this(navProp.GetProperty(), foreignKeyProp.GetProperty(), idProp.GetProperty())
        {
        }

        public ForeignKeyConstraint(PropertyInfo propInfo, PropertyInfo foreignKeyProp, PropertyInfo idProp)
        {
            _foreignKeyProp = foreignKeyProp;
            _idProp = idProp;
            PropInfo = propInfo;
        }

        public void Rebind(T host, TProp previousValue, TProp value)
        {
            var key = _idProp.GetValue(value);
            _foreignKeyProp.SetValue(host, key);
        }

        public PropertyInfo PropInfo { get; }
    }
}
