using FluentAssertions;
using LazyEntityGraph.Core;
using LazyEntityGraph.Core.Constraints;
using LazyEntityGraph.EntityFramework;
using Xunit;

namespace LazyEntityGraph.Tests.EntityFramework
{
    public class EntityFrameworkTests
    {
        private static ModelMetadata GetMetadata()
        {
            return ModelMetadataGenerator.LoadFromCodeFirstContext(str => new BlogContext(str), true);
        }

        [Fact]
        public void EntityTypesShouldBeDetected()
        {
            // arrange & act
            var metadata = GetMetadata();

            // assert
            metadata.EntityTypes.Should()
                .BeEquivalentTo(typeof(Post), typeof(User), typeof(Tag), typeof(ContactDetails), typeof(Category));
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
            var metadata = GetMetadata();

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
            var metadata = GetMetadata();

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
            var metadata = GetMetadata();

            // assert
            metadata.Constraints.Should().Contain(expected);
        }

        [Fact]
        public void ForeignKeyConstraintsShouldBeGenerated()
        {
            // arrange
            var expected = new IPropertyConstraint[]
            {
                new ForeignKeyConstraint<Post, User, int>(p => p.Poster, p => p.PosterId, u => u.Id),
                new ForeignKeyConstraint<ContactDetails, User, int>(c => c.User, c => c.Id, u => u.Id)
            };

            // act
            var metadata = GetMetadata();

            // assert
            metadata.Constraints.Should()
                .Contain(x => x.Equals(expected[0]))
                .And
                .Contain(x => x.Equals(expected[1]));
        }
    }
}
