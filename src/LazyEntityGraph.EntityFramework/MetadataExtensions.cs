using System;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Reflection;

namespace LazyEntityGraph.EntityFramework
{
    static class MetadataExtensions
    {
        public static Type GetClrType(this EntityType entityType, Assembly contextAssembly)
        {
            var edmxV5Type = entityType.MetadataProperties
                .SingleOrDefault(p => p.Name == @"http://schemas.microsoft.com/ado/2013/11/edm/customannotation:ClrType")
                ?.Value as Type;

            if (edmxV5Type != null)
            {
                return edmxV5Type;
            }

            // find ef6 model
            var namespaceName = entityType.MetadataProperties
                .Single(p => p.Name == "NamespaceName")
                .Value;

            var typeName = entityType.MetadataProperties
                .Single(p => p.Name == "Name")
                .Value;

            return contextAssembly.GetType($"{namespaceName}.{typeName}");
        }

        public static PropertyInfo GetProperty(this NavigationProperty navProp, Assembly contextAssembly)
        {
            return navProp
                .FromEndMember
                .GetEntityType()
                .GetClrType(contextAssembly)
                .GetProperty(navProp.Name,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
        }

        public static PropertyInfo GetProperty(this Type t, EdmProperty property)
        {
            return t.GetProperty(property.Name,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
        }
    }
}