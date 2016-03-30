using LazyEntityGraph.Core;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using System;
using System.Collections.Generic;

namespace LazyEntityGraph.AutoFixture
{
    public class LazyEntityGraphCustomization : ICustomization
    {
        private readonly IReadOnlyCollection<Type> _entityTypes;
        private readonly IReadOnlyCollection<IPropertyConstraint> _constraints;

        public LazyEntityGraphCustomization(IReadOnlyCollection<Type> entityTypes, IReadOnlyCollection<IPropertyConstraint> constraints)
        {
            if (entityTypes == null)
                throw new ArgumentNullException(nameof(entityTypes));
            if (constraints == null)
                throw new ArgumentNullException(nameof(constraints));

            _entityTypes = entityTypes;
            _constraints = constraints;
        }

        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Insert(0,
                new FilteringSpecimenBuilder(
                    new Postprocessor(
                        new EntitySpecimenBuilder(_entityTypes, _constraints),
                        new AutoPropertiesCommand(
                            new InverseRequestSpecification(
                                new OrRequestSpecification(
                                    new EntityPropertyCollectionRequestSpecification(_entityTypes),
                                    new InterceptorsFieldRequestSpecification())))),
                    new MatchingTypeRequestSpecification(_entityTypes)));
        }
    }
}
