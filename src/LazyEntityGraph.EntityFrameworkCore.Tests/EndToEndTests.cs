using FluentAssertions;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using LazyEntityGraph.EntityFrameworkCore.Tests.Model;
using LazyEntityGraph.EntityFrameworkCore.Tests.Model.TPH;
using Microsoft.EntityFrameworkCore;
// ReSharper disable InconsistentNaming

namespace LazyEntityGraph.EntityFrameworkCore.Tests
{
    public class EndToEndTests : IDisposable
    {
        private readonly BlogContext _context;

        public EndToEndTests()
        {
            var options = new DbContextOptionsBuilder<BlogContext>()
                .UseInMemoryDatabase(databaseName: "App=EntityFrameworkCore")
                .Options;
            _context = new BlogContext(options);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [Theory, BlogModelData]
        public void ForeignKeyPropertyOnTPHBaseOneToMany(Post post)
        {
            // assert
            post.Poster.Id.Should().Be(post.PosterId);
        }

        [Theory, BlogModelData]
        public void ForeignKeyPropertyOnTPHManyToOne(User user)
        {
            // assert
            foreach (var post in user.Posts)
                post.PosterId.Should().Be(user.Id);
        }

        [Theory, BlogModelData]
        public void ForeignKeyPropertyOnTPHDerivedOneToMany(Story story)
        {
            // assert
            story.Poster.Id.Should().Be(story.PosterId);
        }

        [Theory, BlogModelData]
        public void TPHBaseCanBeAddedToDatabase(Post post)
        {
            // arrange
            var allPosts = new List<Post> { post };
            allPosts.AddRange(post.Poster.Posts);
            allPosts = allPosts.GroupBy(x => x.Id).Select(x => x.First()).ToList();

            // act
            Action act = () =>
            {
                _context.Add(post);
                _context.SaveChanges();
            };

            // assert
            act.Should().NotThrow();
            _context.Posts.ToList().Should().BeEquivalentTo(allPosts);
        }

        [Theory, BlogModelData]
        public void TPHDerivedCanBeAddedToDatabase(Story story)
        {
            // arrange
            var allStories = new List<Story> { story };
            allStories.AddRange(story.Poster.Posts.OfType<Story>());
            allStories = allStories.GroupBy(x => x.Id).Select(x => x.First()).ToList();

            // act
            Action act = () =>
            {
                _context.Add(story);
                _context.SaveChanges();
            };

            // assert
            act.Should().NotThrow();
            _context.Stories.ToList().Should().BeEquivalentTo(allStories);
        }
    }
}
