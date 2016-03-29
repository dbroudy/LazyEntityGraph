using Castle.DynamicProxy;

namespace LazyEntityGraph.Core.Interceptors
{
    public class NullInterceptor : IInterceptor
    {
        private NullInterceptor() { }

        public void Intercept(IInvocation invocation) { }

        public static readonly NullInterceptor Instance = new NullInterceptor();
    }
}
