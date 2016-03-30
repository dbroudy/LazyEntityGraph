using System.Reflection;

namespace LazyEntityGraph.Core
{
    public interface IProperty<T>
    {
        PropertyInfo PropInfo { get; }
        bool IsSet { get; }
        void Set(object value);
        object Get();
    }

    public interface IProperty<T, TProperty> : IProperty<T>
    {
        void Set(TProperty value);
        new TProperty Get();
        bool TryGet(out TProperty value);
    }
}
