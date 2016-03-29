using Castle.DynamicProxy;
using LazyEntityGraph.Core.InterceptionPolicies;
using System;

namespace LazyEntityGraph.Core.Interceptors
{
    public class FilteringInterceptor : IInterceptor
    {
        private readonly IInterceptor _interceptor;
        private readonly IInterceptionPolicy _policy;
        private readonly IInterceptor _elseInterceptor;

        public FilteringInterceptor(IInterceptionPolicy policy, IInterceptor interceptor)
            : this(policy, interceptor, NullInterceptor.Instance)
        { }

        public FilteringInterceptor(IInterceptionPolicy policy, IInterceptor interceptor, IInterceptor elseInterceptor)
        {
            if (policy == null)
                throw new ArgumentNullException(nameof(policy));
            if (interceptor == null)
                throw new ArgumentNullException(nameof(interceptor));
            if (elseInterceptor == null)
                throw new ArgumentNullException(nameof(elseInterceptor));

            _interceptor = interceptor;
            _policy = policy;
            _elseInterceptor = elseInterceptor;
        }

        public void Intercept(IInvocation invocation)
        {
            if (_policy.ShouldIntercept(invocation))
                _interceptor.Intercept(invocation);
            else
                _elseInterceptor.Intercept(invocation);
        }
    }
}
