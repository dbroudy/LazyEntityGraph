using System.Reflection;

namespace LazyEntityGraph.Core
{
    public interface IPropertyAccessor<out T>
    {
        IProperty<TProp> Get<TProp>(PropertyInfo propInfo);
    }
}
