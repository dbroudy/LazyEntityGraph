using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using System;
using System.Collections.Generic;

namespace LazyEntityGraph.AutoFixture
{
    public class LazyEntityGraphCustomization : ICustomization
    {
        private readonly IReadOnlyCollection<Type> _entityTypes;

        public LazyEntityGraphCustomization(IReadOnlyCollection<Type> entityTypes)
        {
            _entityTypes = entityTypes;
        }

        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Insert(0,
                new FilteringSpecimenBuilder(
                    new Postprocessor(
                        new EntitySpecimenBuilder(),
                        new AutoPropertiesCommand(
                            new InverseRequestSpecification(
                                new OrRequestSpecification(
                                    new EntityPropertyRequestSpecification(_entityTypes),
                                    new InterceptorsFieldRequestSpecification())))),
                    new MatchingTypeRequestSpecification(_entityTypes)));
        }
    }
}
