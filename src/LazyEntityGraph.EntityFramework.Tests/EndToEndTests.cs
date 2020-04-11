using FluentAssertions;
using LazyEntityGraph.AutoFixture;
using AutoFixture;
using AutoFixture.Xunit2;
using Xunit;
using System;

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

    public class EndToEndTests
    {
        [Theory, BlogModelData]
        public void ForeignKeyPropertyOnOneToMany(Post post)
        {
            var posterId = post.PosterId;
            // assert
            post.Poster.Id.Should().Be(posterId);
        }

        [Theory, BlogModelData]
        public void ForeignKeyPropertyOnManyToOne(User user)
        {
            // assert
            foreach (var post in user.Posts)
                post.PosterId.Should().Be(user.Id);
        }

        [Theory, BlogModelData]
        public void ForeignKeyPropertyOnDerivedOneToManyObjectFirst(Story story)
        {
            // assert
            story.Poster.Id.Should().Be(story.PosterId);
        }

        [Theory, BlogModelData]
        public void ForeignKeyPropertyOnDerivedOneToManyIdFirst(Story story)
        {
            var posterId = story.PosterId;
            // assert
            story.Poster.Id.Should().Be(posterId);
        }

        [Theory, BlogModelData]
        public void ForeignKeyPropertyOnOneToOne(User user)
        {
            // assert
            user.Id.Should().Be(user.ContactDetails.UserId);
        }

        [Theory, BlogModelData]
        public void ForeignKeyPropertyOnOneToOneInverse(ContactDetails contact)
        {
            var userId = contact.UserId;
            // assert
            userId.Should().Be(contact.User.Id);
        }
    }
}
