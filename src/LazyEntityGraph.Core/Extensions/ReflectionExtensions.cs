using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LazyEntityGraph.Core.Extensions
{
    static class ReflectionExtensions
    {
        public static PropertyInfo GetProperty<T, TProp>(this Expression<Func<T, TProp>> expr)
        {
            return (PropertyInfo)((MemberExpression)expr.Body).Member;
        }

        public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = givenType.BaseType;
            if (baseType == null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }

        public static PropertyInfo GetParentProperty(this MethodInfo method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            var takesArg = method.GetParameters().Length == 1;
            var hasReturn = method.ReturnType != typeof(void);
            if (!(takesArg || hasReturn)) return null;

            if (takesArg && !hasReturn)
            {
                return method.DeclaringType.GetProperties().FirstOrDefault(prop => prop.GetSetMethod() == method);
            }
            else
            {
                return method.DeclaringType.GetProperties().FirstOrDefault(prop => prop.GetGetMethod() == method);
            }
        }

        public static bool PropertyEquals(this PropertyInfo x, PropertyInfo y)
        {
            if (x == y)
                return true;
            if (x == null || y == null)
                return false;
            if (x.MetadataToken != y.MetadataToken)
                return false;
            if (x.Name != y.Name)
                return false;
            if (x.PropertyType != y.PropertyType)
                return false;
            if (x.DeclaringType != y.DeclaringType)
                return false;
            return true;
        }
    }
}
