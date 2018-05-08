using LazyEntityGraph.Core;
using AutoFixture;
using AutoFixture.Kernel;
using System;

namespace LazyEntityGraph.AutoFixture
{
    public class LazyEntityGraphCustomization : ICustomization
    {
        private readonly ModelMetadata _modelMetadata;

        public LazyEntityGraphCustomization(ModelMetadata modelMetadata)
        {
            if (modelMetadata == null)
                throw new ArgumentNullException(nameof(modelMetadata));

            _modelMetadata = modelMetadata;
        }

        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Insert(0,
                new FilteringSpecimenBuilder(
                    new Postprocessor(
                        new EntitySpecimenBuilder(_modelMetadata),
                        new AutoPropertiesCommand(
                            new InverseRequestSpecification(
                                new OrRequestSpecification(
                                    new EntityPropertyCollectionRequestSpecification(_modelMetadata.EntityTypes),
                                    new InterceptorsFieldRequestSpecification())))),
                    new MatchingTypeRequestSpecification(_modelMetadata.EntityTypes)));
        }
    }
}
