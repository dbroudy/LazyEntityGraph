using Castle.DynamicProxy;

namespace LazyEntityGraph.Core.InterceptionPolicies
{
    public interface IInterceptionPolicy
    {
        bool ShouldIntercept(IInvocation invocation);
    }
}
