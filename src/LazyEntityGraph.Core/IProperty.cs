using System.Reflection;

namespace LazyEntityGraph.Core
{
    public interface IProperty
    {
        PropertyInfo PropInfo { get; }
        void Set(object value);
        object Get();
    }

    public interface IProperty<TProperty> : IProperty
    {
        void Set(TProperty value);
        new TProperty Get();
        bool TryGet(out TProperty value);
    }
}
