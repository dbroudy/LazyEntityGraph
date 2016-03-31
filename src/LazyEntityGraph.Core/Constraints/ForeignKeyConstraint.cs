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

        public ForeignKeyConstraint(Expression<Func<T, TProp>> navProp, Expression<Func<T, TKey>> foreignKeyProp,
            Expression<Func<TProp, TKey>> idProp)
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

        protected bool Equals(ForeignKeyConstraint<T, TProp, TKey> other)
        {
            return Equals(_foreignKeyProp, other._foreignKeyProp) && Equals(_idProp, other._idProp) &&
                   Equals(PropInfo, other.PropInfo);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((ForeignKeyConstraint<T, TProp, TKey>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_foreignKeyProp != null ? _foreignKeyProp.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_idProp != null ? _idProp.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PropInfo != null ? PropInfo.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}