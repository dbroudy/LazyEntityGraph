using System;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Reflection;

namespace LazyEntityGraph.EntityFramework
{
    static class MetadataExtensions
    {
        public static Type GetClrType(this EntityType entityType)
        {
            return (Type)entityType.MetadataProperties
                .Single(p => p.Name == @"http://schemas.microsoft.com/ado/2013/11/edm/customannotation:ClrType")
                .Value;
        }

        public static PropertyInfo GetProperty(this NavigationProperty navProp)
        {
            return navProp
                .FromEndMember
                .GetEntityType()
                .GetClrType()
                .GetProperty(navProp.Name,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
        }
    }
}