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
            var metadataWorkspace = MetadataWorkspaceFactory<TContext>.GetMetadataWorkspaceFromEdmx(contextName);
            return GenerateModelMetadata(metadataWorkspace);
        }

        public static ModelMetadata LoadFromCodeFirstContext<TContext>(Func<string, TContext> createFromConnectionString)
            where TContext : DbContext
        {
            var metadataWorkspace = MetadataWorkspaceFactory<TContext>.GetMetadataWorkspaceFromCodeFirst(createFromConnectionString);
            return GenerateModelMetadata(metadataWorkspace);
        }

        private static ModelMetadata GenerateModelMetadata(MetadataWorkspace ws)
        {
            var entityTypes = ws.GetItems<EntityType>(DataSpace.CSpace);

            var types = entityTypes.Select(x => x.GetClrType());
            var constraints = entityTypes
                .SelectMany(et => et.NavigationProperties)
                .GroupBy(np => np.RelationshipType)
                .Select(r => r.ToList())
                .Where(r => r.Count == 2)
                .SelectMany(r => GetConstraints(r[0], r[1]));

            return new ModelMetadata(types, constraints);
        }

        private static IPropertyConstraint CreateGenericConstraint(Type openGeneric, PropertyInfo a, PropertyInfo b)
        {
            var closedGeneric = openGeneric.MakeGenericType(a.DeclaringType, b.DeclaringType);
            return (IPropertyConstraint)Activator.CreateInstance(closedGeneric, a, b);
        }

        private static IEnumerable<IPropertyConstraint> GetConstraints(NavigationProperty from, NavigationProperty to)
        {
            var fromProp = from.GetProperty();
            var toProp = to.GetProperty();
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
        }
    }
}