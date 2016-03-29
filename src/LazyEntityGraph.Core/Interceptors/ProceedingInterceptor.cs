using Castle.DynamicProxy;
using System;

namespace LazyEntityGraph.Core.Interceptors
{
    public class ProceedingInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (invocation == null)
                throw new ArgumentNullException(nameof(invocation));

            invocation.Proceed();
        }
    }
}
