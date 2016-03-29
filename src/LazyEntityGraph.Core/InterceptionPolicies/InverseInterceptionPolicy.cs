using Castle.DynamicProxy;
using System;

namespace LazyEntityGraph.Core.InterceptionPolicies
{
    public class InverseInterceptionPolicy : IInterceptionPolicy
    {
        private readonly IInterceptionPolicy _wrapped;

        public InverseInterceptionPolicy(IInterceptionPolicy wrapped)
        {
            if (wrapped == null)
                throw new ArgumentNullException(nameof(wrapped));

            _wrapped = wrapped;
        }

        public bool ShouldIntercept(IInvocation invocation)
        {
            if (invocation == null)
                throw new ArgumentNullException(nameof(invocation));

            return !_wrapped.ShouldIntercept(invocation);
        }
    }
}
