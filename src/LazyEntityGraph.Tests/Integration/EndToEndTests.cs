using FluentAssertions;
using LazyEntityGraph.AutoFixture;
using LazyEntityGraph.EntityFramework;
using LazyEntityGraph.Tests.EntityFramework;
using AutoFixture;
using AutoFixture.Xunit2;
using Xunit;
using System;

namespace LazyEntityGraph.Tests.Integration
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

        [Theory, BlogModelData]
        public void ForeignKeyPropertyOnDerivedOneToMany(Story story)
        {
            // assert
            story.Poster.Id.Should().Be(story.PosterId);
        }
    }
}
