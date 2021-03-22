using LazyEntityGraph.Core.Extensions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LazyEntityGraph.Core.Constraints
{
    public class ForeignKeyConstraint<T, TProp> : IPropertyConstraint<T, TProp>
        where T : class
        where TProp : class
    {
        private readonly PropertyInfo[] _foreignKeyProps;
        private readonly PropertyInfo[] _idProps;

        public ForeignKeyConstraint(PropertyInfo propInfo, PropertyInfo[] foreignKeyProps, PropertyInfo[] idProp)
        {
            _foreignKeyProps = foreignKeyProps;
            _idProps = idProp;
            PropInfo = propInfo;
        }

        public void Rebind(T host, TProp previousValue, TProp value)
        {
            for (var idx = 0; idx < _idProps.Length; idx++)
            {
                var key = value == null ? null : _idProps[idx].GetValue(value);
                _foreignKeyProps[idx].SetValue(host, key);
            }
        }

        public PropertyInfo PropInfo { get; }

        #region Equality
        protected bool Equals(ForeignKeyConstraint<T, TProp> other)
        {
            return _foreignKeyProps.SequenceEqual(other._foreignKeyProps, new PropertyComparer())
                   && _idProps.SequenceEqual(other._idProps, new PropertyComparer())
                   && PropInfo.PropertyEquals(other.PropInfo);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((ForeignKeyConstraint<T, TProp>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_foreignKeyProps != null ? _foreignKeyProps.Aggregate(0, (a, p) => a ^ p.GetHashCode()) : 0);
                hashCode = (hashCode * 397) ^ (_idProps != null ? _idProps.Aggregate(0, (a, p) => a ^ p.GetHashCode()) : 0);
                hashCode = (hashCode * 397) ^ (PropInfo != null ? PropInfo.GetHashCode() : 0);
                return hashCode;
            }
        }
        #endregion

        public override string ToString()
        {
            return $@"{typeof(T).Name}.{PropInfo.Name}({
                string.Join(",", _foreignKeyProps.Select(p => p.Name))}) references {typeof(TProp).Name}.{
                string.Join(",", _idProps.Select(p => p.Name))}";
        }
    }
}