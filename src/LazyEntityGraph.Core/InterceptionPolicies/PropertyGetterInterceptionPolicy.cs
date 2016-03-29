using Castle.DynamicProxy;
using System;

namespace LazyEntityGraph.Core.InterceptionPolicies
{
    public class PropertyGetterInterceptionPolicy : IInterceptionPolicy
    {
        public bool ShouldIntercept(IInvocation invocation)
        {
            if (invocation == null)
                throw new ArgumentNullException(nameof(invocation));

            return invocation.Method.IsSpecialName
                && invocation.Method.ReturnType != typeof(void);
        }
    }
}
