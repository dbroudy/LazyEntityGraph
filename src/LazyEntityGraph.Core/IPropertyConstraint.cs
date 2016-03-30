using System.Reflection;

namespace LazyEntityGraph.Core
{
    public interface IPropertyConstraint<in THost, in TProperty> : IPropertyConstraint
    {
        void Bind(THost host, TProperty value);
    }

    public interface IPropertyConstraint
    {
        PropertyInfo PropInfo { get; }
    }
}