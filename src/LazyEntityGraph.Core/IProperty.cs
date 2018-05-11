using System.Reflection;

namespace LazyEntityGraph.Core
{
    public interface IProperty<out T>
    {
        PropertyInfo PropInfo { get; }
        void Set(object value);
        object Get();
    }

    public interface IProperty<out T, TProperty> : IProperty<T>
    {
        void Set(TProperty value);
        new TProperty Get();
        bool TryGet(out TProperty value);
    }
}
