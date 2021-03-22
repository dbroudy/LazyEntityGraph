using FluentAssertions;
using LazyEntityGraph.AutoFixture;
using AutoFixture;
using AutoFixture.Xunit2;
using Xunit;
using System;

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
        public void ForeignKeyPropertyOnOneSided(User user)
        {
            // assert
            user.DefaultCategory.Id.Should().Be(user.DefaultCategoryId);
        }

        [Theory, BlogModelData]
        public void ForeignKeyPropertyOnDerivedOneToManyObjectFirst(Story story)
        {
            var relatedPosterId = story.Poster.Id;
            var posterId = story.PosterId;
            // assert
            posterId.Should().Be(relatedPosterId);
        }

        [Theory, BlogModelData]
        public void ForeignKeyPropertyOnDerivedOneToManyIdFirst(Story story)
        {
            var posterId = story.PosterId;
            var relatedPosterId = story.Poster.Id;
            // assert
            relatedPosterId.Should().Be(posterId);
        }

        [Theory, BlogModelData]
        public void ForeignKeyPropertyOnOneToOne(User user)
        {
            var userId = user.Id;
            var relatedUserId = user.ContactDetails.UserId;
            // assert
            relatedUserId.Should().Be(userId);
        }

        [Theory, BlogModelData]
        public void ForeignKeyPropertyOnOneToOneInverse(ContactDetails contact)
        {
            var userId = contact.UserId;
            var relatedUserId = contact.User.Id;
            // assert
            relatedUserId.Should().Be(userId);
        }
    }
}
