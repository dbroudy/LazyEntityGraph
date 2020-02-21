using Castle.DynamicProxy;
using LazyEntityGraph.Core.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LazyEntityGraph.Core
{
    public interface IPropertyAccessor<out T>
    {
        IProperty<TProp> Get<TProp>(PropertyInfo propInfo);
    }

    class VirtualPropertyInterceptor<T> : IInterceptor, IPropertyAccessor<T>
    {
        private IList<IProperty> _properties;

        public void SetProperties(IEnumerable<IProperty> properties)
        {
            _properties = properties.ToList();
        }

        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.DeclaringType == typeof(IPropertyAccessor<T>))
            {
                invocation.ReturnValue = invocation.Method.Invoke(this, invocation.Arguments);
                return;
            }

            if (_properties == null)
            {
                invocation.Proceed();
                return;
            }

            var propInfo = invocation.Method.GetParentProperty();
            if (propInfo == null)
            {
                invocation.Proceed();
                return;
            }

            IProperty property = _properties.SingleOrDefault(p => p.PropInfo.PropertyEquals(propInfo));
            if (property == null)
            {
                invocation.Proceed();
                return;
            }

            var isGetter = invocation.Method.ReturnType != typeof(void);
            if (isGetter)
            {
                invocation.ReturnValue = property.Get();
            }
            else
            {
                property.Set(invocation.Arguments[0]);
            }
        }

        public IProperty<TProp> Get<TProp>(PropertyInfo propInfo)
        {
            return (IProperty<TProp>)_properties.SingleOrDefault(p => p.PropInfo.PropertyEquals(propInfo));
        }
    }
}
