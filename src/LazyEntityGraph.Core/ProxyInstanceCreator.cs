using Castle.DynamicProxy;
using LazyEntityGraph.Core.Interceptors;
using System;

namespace LazyEntityGraph.Core
{
    public class ProxyInstanceCreator : IInstanceCreator
    {
        private readonly IInstanceCreator _fallback;
        private readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();

        public ProxyInstanceCreator(IInstanceCreator fallback)
        {
            if (fallback == null)
                throw new ArgumentNullException(nameof(fallback));

            _fallback = fallback;
        }

        public object Create(Type type)
        {
            var interceptor = GetInterceptor();
            var instance = _proxyGenerator.CreateClassProxy(type, interceptor);
            return instance;
        }

        private IInterceptor GetInterceptor()
        {
            return NullInterceptor.Instance;
        }
    }
}
