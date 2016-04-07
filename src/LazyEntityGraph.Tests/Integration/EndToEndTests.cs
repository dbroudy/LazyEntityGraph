using FluentAssertions;
using LazyEntityGraph.AutoFixture;
using LazyEntityGraph.EntityFramework;
using LazyEntityGraph.Tests.EntityFramework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace LazyEntityGraph.Tests.Integration
{
    public class BlogModelDataAttribute : AutoDataAttribute
    {
        public BlogModelDataAttribute()
            : base(new Fixture()
                  .Customize(new LazyEntityGraphCustomization(
                      ModelMetadataGenerator.LoadFromCodeFirstContext(str => new BlogContext(str), true))))
        {

        }
    }

    public class EndToEndTests
    {
        [Theory, BlogModelData]
        public void ForeignKeyPropertyOnOneToMany(Post post)
        {
            // assert
            post.Poster.Id.Should().Be(post.PosterId);
        }

        [Theory, BlogModelData]
        public void ForeignKeyPropertyOnManyToOne(User user)
        {
            // assert
            foreach (var post in user.Posts)
                post.PosterId.Should().Be(user.Id);
        }
    }
}
