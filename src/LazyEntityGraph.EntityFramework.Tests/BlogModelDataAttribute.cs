using AutoFixture;
using AutoFixture.Xunit2;
using LazyEntityGraph.AutoFixture;
using LazyEntityGraph.EntityFramework.Tests.Model;

namespace LazyEntityGraph.EntityFramework.Tests
{
    public class BlogModelDataAttribute : AutoDataAttribute
    {
        public BlogModelDataAttribute() : base(CreateFixture)
        {
        }

        private static IFixture CreateFixture()
        {
            return new Fixture()
                .Customize(new LazyEntityGraphCustomization(
                    ModelMetadataGenerator.LoadFromCodeFirstContext(str => new BlogContext(str), false)));
        }
    }
}