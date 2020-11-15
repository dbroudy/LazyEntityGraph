using FluentAssertions;
using LazyEntityGraph.Core;
using LazyEntityGraph.Core.Constraints;
using LazyEntityGraph.EntityFrameworkCore;
using LazyEntityGraph.TestUtils;
using System.Collections;
using Xunit;

namespace LazyEntityGraph.EntityFrameworkCore.Tests
{
    public class ModelMetadataGeneratorTests
    {
        public static ModelMetadata GetMetadata()
        {
            return ModelMetadataGenerator.LoadFromContext<BlogContext>(options => new BlogContext(options));
        }

        [Fact]
        public void EntityTypesShouldBeDetected()
        {
            // arrange
            var expected = new[]
            {
                typeof (Post), typeof (User), typeof (ContactDetails), typeof (Category), typeof(Story)
            };

            // act
            var metadata = GetMetadata();

            // assert
            metadata.EntityTypes.Should().BeEquivalentTo(expected);
        }


        [Fact]
        public void ConstraintsShouldBeGenerated()
        {
            // arrange
            var expected = new IPropertyConstraint[]
            {
                ExpectedConstraints.CreateOneToMany<User, Post>(u => u.Posts, p => p.Poster),
                ExpectedConstraints.CreateManyToOne<Post, User>(p => p.Poster, u => u.Posts),
                ExpectedConstraints.CreateOneToOne<User, ContactDetails>(u => u.ContactDetails, c => c.User),
                ExpectedConstraints.CreateOneToOne<ContactDetails, User>(c => c.User, u => u.ContactDetails),
                ExpectedConstraints.CreateForeignKey<Post, User, int>(p => p.Poster, p => p.PosterId, u => u.Id),
                ExpectedConstraints.CreateForeignKey<ContactDetails, User, int>(c => c.User, c => c.UserId, u => u.Id)
            };

            // act
            var metadata = GetMetadata();

            // assert
            metadata.Constraints.Should().BeEquivalentTo(expected);
        }
    }
}
