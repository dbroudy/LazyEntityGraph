using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LazyEntityGraph.Core.InterceptionPolicies
{
    public class OrInterceptionPolicy : IInterceptionPolicy
    {
        private readonly IEnumerable<IInterceptionPolicy> _policies;

        public OrInterceptionPolicy(params IInterceptionPolicy[] policies)
            : this((IEnumerable<IInterceptionPolicy>)policies)
        { }

        public OrInterceptionPolicy(IEnumerable<IInterceptionPolicy> policies)
        {
            if (policies == null)
                throw new ArgumentNullException(nameof(policies));

            _policies = policies;
        }

        public bool ShouldIntercept(IInvocation invocation)
        {
            if (invocation == null)
                throw new ArgumentNullException(nameof(invocation));

            return _policies.Any(x => x.ShouldIntercept(invocation));
        }
    }
}
