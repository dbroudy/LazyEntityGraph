using FluentAssertions;
using LazyEntityGraph.Core;
using LazyEntityGraph.Core.Constraints;
using LazyEntityGraph.EntityFramework;
using System.Collections;
using Xunit;

namespace LazyEntityGraph.EntityFramework.Tests
{
    public class ModelMetadataGeneratorTests
    {
        public static ModelMetadata GetMetadata()
        {
            return ModelMetadataGenerator.LoadFromCodeFirstContext(str => new BlogContext(str), false);
        }

        [Fact]
        public void EntityTypesShouldBeDetected()
        {
            // arrange
            var expected = new[]
            {
                typeof (Post), typeof (User), typeof (Tag), typeof (ContactDetails), typeof (Category), typeof(Story)
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
                new ManyToManyPropertyConstraint<Post, Tag>(p => p.Tags, t => t.Posts),
                new ManyToManyPropertyConstraint<Tag, Post>(t => t.Posts, p => p.Tags),
                new OneToManyPropertyConstraint<User, Post>(u => u.Posts, p => p.Poster),
                new ManyToOnePropertyConstraint<Post, User>(p => p.Poster, u => u.Posts),
                new OneToOnePropertyConstraint<User,ContactDetails>(u => u.ContactDetails, c => c.User),
                new OneToOnePropertyConstraint<ContactDetails, User>(c => c.User, u => u.ContactDetails),
                ForeignKeyConstraint<Post, User>.Create(p => p.Poster, p => p.PosterId, u => u.Id),
                ForeignKeyConstraint<ContactDetails, User>.Create(c => c.User, c => c.UserId, u => u.Id)
            };

            // act
            var metadata = GetMetadata();

            // assert
            metadata.Constraints.Should().BeEquivalentTo(expected);
        }
    }
}
