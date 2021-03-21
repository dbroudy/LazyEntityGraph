﻿using LazyEntityGraph.Core;
using LazyEntityGraph.Core.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LazyEntityGraph.EntityFrameworkCore
{
    public static class ModelMetadataGenerator
    {
        public static ModelMetadata LoadFromContext<TContext>(Func<DbContextOptions<TContext>, TContext> createContext)
            where TContext : DbContext
        {
            var options = new DbContextOptionsBuilder<TContext>()
                .UseInMemoryDatabase(databaseName: "App=EntityFrameworkCore")
                .Options;

            using (var ctx = createContext(options))
                return GenerateModelMetadata(ctx);
        }

        private static ModelMetadata GenerateModelMetadata(DbContext ctx)
        {
            var entityTypes = ctx.Model.GetEntityTypes();

            var types = entityTypes.Select(x => x.ClrType);
            var constraints = entityTypes
                .SelectMany(et => et.GetNavigations())
                .SelectMany(r => GetConstraints(r.FindInverse(), r));

            return new ModelMetadata(types, constraints);
        }

        private static IPropertyConstraint CreateGenericConstraint(Type openGeneric, PropertyInfo a, PropertyInfo b)
        {
            var closedGeneric = openGeneric.MakeGenericType(a.DeclaringType, b.DeclaringType);
            return (IPropertyConstraint)Activator.CreateInstance(closedGeneric, a, b);
        }

        private static IEnumerable<IPropertyConstraint> GetConstraints(INavigation from, INavigation to)
        {
            var fromProp = from?.PropertyInfo;
            var toProp = to?.PropertyInfo;
            var fromMultiplicity = from?.IsCollection() ?? false;
            var toMultiplicity = to?.IsCollection() ?? false;

            if (fromProp != null && toProp != null)
            {
                if (toMultiplicity)
                {
                    yield return CreateGenericConstraint(typeof(ManyToOnePropertyConstraint<,>), fromProp, toProp);
                    yield return CreateGenericConstraint(typeof(OneToManyPropertyConstraint<,>), toProp, fromProp);
                }
                else if (fromMultiplicity)
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

            var fromForeignKey = GetForeignKeyConstraint(from);
            if (fromForeignKey != null)
                yield return fromForeignKey;

            var toForeignKey = GetForeignKeyConstraint(to);
            if (toForeignKey != null)
                yield return toForeignKey;
        }

        private static IPropertyConstraint GetForeignKeyConstraint(INavigation navProp)
        {
            if (navProp == null)
                return null;

            if (!navProp.IsDependentToPrincipal() || navProp.IsCollection())
                return null;

            var fkProp = navProp.ForeignKey;

            var fromType = navProp.DeclaringEntityType.ClrType;
            var toType = fkProp.PrincipalEntityType.ClrType;

            // Use IPropertyBase because it works in EF Core 2.1, where ClrType is defined in IPropertyBase and IProperty and EF Core 3.1 where it's only defined in IPropertyBase
            IPropertyBase fkPrincipalProp = fkProp.PrincipalKey.Properties.Single();
            var foreignKeyPropInfo = fkProp.Properties.Single().PropertyInfo;
            var keyPropInfo = fkPrincipalProp.PropertyInfo;
            var keyType = fkPrincipalProp.ClrType;

            var type = typeof(ForeignKeyConstraint<,,>)
                .MakeGenericType(fromType, toType, keyType);
            return (IPropertyConstraint)Activator.CreateInstance(type, navProp.PropertyInfo, foreignKeyPropInfo, keyPropInfo);
        }
    }
}