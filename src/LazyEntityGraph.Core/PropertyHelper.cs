using System.Reflection;

namespace LazyEntityGraph.Core
{
    public static class PropertyHelper
    {
        public static void Set<T, TValue>(T obj, PropertyInfo pi, TValue value)
            where T : class
            where TValue : class
        {
            if (obj == null)
                return;

            var propertyAccessor = obj as IPropertyAccessor<T>;
            if (propertyAccessor == null)
            {
                // property object is not proxy but we can still set the value
                pi.SetValue(obj, value);
                return;
            }

            var property = propertyAccessor.Get<TValue>(pi);
            if (property == null)
                return;

            TValue existing;
            if (property.TryGet(out existing) && existing == value)
                return;

            property.Set(value);
        }
    }
}
