using FluentAssertions;
using LazyEntityGraph.Core.Constraints;
using LazyEntityGraph.EntityFramework;
using Xunit;

namespace LazyEntityGraph.Tests.EntityFramework
{
    public class EntityFrameworkTests
    {
        [Fact]
        public void EntityTypesShouldBeDetected()
        {
            // arrange & act
            var metadata = ModelMetadataGenerator.LoadFromCodeFirstContext(str => new BlogContext(str));

            // assert
            metadata.EntityTypes.Should()
                .BeEquivalentTo(typeof(Post), typeof(User), typeof(Tag), typeof(ContactDetails));
        }

        [Fact]
        public void ManyToManyConstraintsShouldBeGenerated()
        {
            // arrange
            var expected = new IPropertyConstraint[]
            {
                new ManyToManyPropertyConstraint<Post, Tag>(p => p.Tags, t => t.Posts),
                new ManyToManyPropertyConstraint<Tag, Post>(t => t.Posts, p => p.Tags)
            };

            // act
            var metadata = ModelMetadataGenerator.LoadFromCodeFirstContext(str => new BlogContext(str));

            // assert
            metadata.Constraints.Should().Contain(expected);
        }

        [Fact]
        public void OneToManyConstraintsShouldBeGenerated()
        {
            // arrange
            var expected = new IPropertyConstraint[]
            {
                new OneToManyPropertyConstraint<User, Post>(u => u.Posts, p => p.Poster),
                new ManyToOnePropertyConstraint<Post, User>(p => p.Poster, u => u.Posts)
            };

            // act
            var metadata = ModelMetadataGenerator.LoadFromCodeFirstContext(str => new BlogContext(str));

            // assert
            metadata.Constraints.Should().Contain(expected);
        }

        [Fact]
        public void OneToOneConstraintsShouldBeGenerated()
        {
            // arrange
            var expected = new IPropertyConstraint[]
            {
                new OneToOnePropertyConstraint<User,ContactDetails>(u => u.ContactDetails, c => c.User),
                new OneToOnePropertyConstraint<ContactDetails, User>(c => c.User, u => u.ContactDetails)
            };

            // act
            var metadata = ModelMetadataGenerator.LoadFromCodeFirstContext(str => new BlogContext(str));

            // assert
            metadata.Constraints.Should().Contain(expected);
        }
    }
}
