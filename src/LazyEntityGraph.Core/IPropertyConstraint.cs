using System.Reflection;

namespace LazyEntityGraph.Core
{
    public interface IPropertyConstraint<in THost, in TProperty> : IPropertyConstraint
    {
        void Rebind(THost host, TProperty previousValue, TProperty value);
    }

    public interface IPropertyConstraint
    {
        PropertyInfo PropInfo { get; }
    }
}