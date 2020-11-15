using LazyEntityGraph.Core;
using LazyEntityGraph.Core.Constraints;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Reflection;

namespace LazyEntityGraph.EntityFramework
{
    public static class ModelMetadataGenerator
    {
        public static ModelMetadata LoadFromEdmxContext<TContext>(string contextName) where TContext : DbContext
        {
            var metadataWorkspace = EdmxMetadataWorkspaceFactory<TContext>.GetMetadataWorkspace(contextName);
            return GenerateModelMetadata(metadataWorkspace, typeof(TContext).Assembly);
        }

        public static ModelMetadata LoadFromCodeFirstContext<TContext>(Func<string, TContext> createFromConnectionString, bool hardCache = true)
            where TContext : DbContext
        {
            var metadataWorkspace = CodeFirstMetadataWorkspaceFactory<TContext>.GetMetadataWorkspace(createFromConnectionString, hardCache);
            return GenerateModelMetadata(metadataWorkspace, typeof(TContext).Assembly);
        }

        private static ModelMetadata GenerateModelMetadata(MetadataWorkspace ws, Assembly contextAssembly)
        {
            var entityTypes = ws.GetItems<EntityType>(DataSpace.CSpace);

            var types = entityTypes.Select(x => x.GetClrType(contextAssembly));
            var constraints = entityTypes
                .SelectMany(et => et.DeclaredNavigationProperties)
                .GroupBy(np => np.RelationshipType)
                .Select(r => r.ToList())
                .Where(r => r.Count == 2)
                .SelectMany(r => GetConstraints(r[0], r[1], contextAssembly));

            return new ModelMetadata(types, constraints);
        }

        private static IPropertyConstraint CreateGenericConstraint(Type openGeneric, PropertyInfo a, PropertyInfo b)
        {
            var closedGeneric = openGeneric.MakeGenericType(a.DeclaringType, b.DeclaringType);
            return (IPropertyConstraint)Activator.CreateInstance(closedGeneric, a, b);
        }

        private static IEnumerable<IPropertyConstraint> GetConstraints(NavigationProperty from, NavigationProperty to, Assembly contextAssembly)
        {
            var fromProp = from.GetProperty(contextAssembly);
            var toProp = to.GetProperty(contextAssembly);
            var fromMultiplicity = from.FromEndMember.RelationshipMultiplicity;
            var toMultiplicity = from.ToEndMember.RelationshipMultiplicity;

            if (fromMultiplicity == RelationshipMultiplicity.Many && toMultiplicity == RelationshipMultiplicity.Many)
            {
                yield return CreateGenericConstraint(typeof(ManyToManyPropertyConstraint<,>), fromProp, toProp);
                yield return CreateGenericConstraint(typeof(ManyToManyPropertyConstraint<,>), toProp, fromProp);
            }
            else if (fromMultiplicity == RelationshipMultiplicity.Many)
            {
                yield return CreateGenericConstraint(typeof(ManyToOnePropertyConstraint<,>), fromProp, toProp);
                yield return CreateGenericConstraint(typeof(OneToManyPropertyConstraint<,>), toProp, fromProp);
            }
            else if (toMultiplicity == RelationshipMultiplicity.Many)
            {
                yield return CreateGenericConstraint(typeof(OneToManyPropertyConstraint<,>), fromProp, toProp);
                yield return CreateGenericConstraint(typeof(ManyToOnePropertyConstraint<,>), toProp, fromProp);
            }
            else
            {
                yield return CreateGenericConstraint(typeof(OneToOnePropertyConstraint<,>), fromProp, toProp);
                yield return CreateGenericConstraint(typeof(OneToOnePropertyConstraint<,>), toProp, fromProp);
            }

            var fromForeignKey = GetForeignKeyConstraint(from, contextAssembly);
            if (fromForeignKey != null)
                yield return fromForeignKey;

            var toForeignKey = GetForeignKeyConstraint(to, contextAssembly);
            if (toForeignKey != null)
                yield return toForeignKey;
        }

        private static IPropertyConstraint GetForeignKeyConstraint(NavigationProperty navProp, Assembly contextAssembly)
        {
            if (navProp.GetDependentProperties().Count() != 1
                || navProp.ToEndMember.GetEntityType().KeyProperties.Count != 1)
                return null;

            var dependencyProp = navProp.GetDependentProperties().Single();
            var keyProp = navProp.ToEndMember.GetEntityType().KeyProperties.Single();
            var fromType = navProp.FromEndMember.GetEntityType().GetClrType(contextAssembly);
            var toType = navProp.ToEndMember.GetEntityType().GetClrType(contextAssembly);

            var navPropInfo = navProp.GetProperty(contextAssembly);
            var foreignKeyPropInfo = fromType.GetProperty(dependencyProp);
            var keyPropInfo = toType.GetProperty(keyProp);
            var keyType = keyPropInfo.PropertyType;

            var type = typeof(ForeignKeyConstraint<,>)
                .MakeGenericType(fromType, toType);
            return (IPropertyConstraint)Activator.CreateInstance(type, navPropInfo, new[] { foreignKeyPropInfo }, new [] { keyPropInfo });
        }
    }
}