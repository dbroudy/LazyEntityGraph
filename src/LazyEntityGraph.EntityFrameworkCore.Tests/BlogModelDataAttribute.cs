using AutoFixture;
using AutoFixture.Xunit2;
using LazyEntityGraph.AutoFixture;
using LazyEntityGraph.EntityFrameworkCore.Tests.Model;

namespace LazyEntityGraph.EntityFrameworkCore.Tests
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
                    ModelMetadataGenerator.LoadFromContext<BlogContext>(options => new BlogContext(options))));
        }
    }
}