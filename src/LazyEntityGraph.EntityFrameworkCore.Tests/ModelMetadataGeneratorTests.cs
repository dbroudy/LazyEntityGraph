using FluentAssertions;
using LazyEntityGraph.Core;
using LazyEntityGraph.Core.Constraints;
using System.Linq;
using LazyEntityGraph.EntityFrameworkCore.Tests.Model;
using LazyEntityGraph.EntityFrameworkCore.Tests.Model.TPH;
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
                typeof (Post), typeof (PostLocalized), typeof (User), typeof (ContactDetails), typeof (Category),
                typeof (CategoryLocalized), typeof (Story),
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
                new OneToManyPropertyConstraint<User, Post>(u => u.Posts, p => p.Poster),
                new ManyToOnePropertyConstraint<Post, User>(p => p.Poster, u => u.Posts),
                new OneToOnePropertyConstraint<User,ContactDetails>(u => u.ContactDetails, c => c.User),
                new OneToOnePropertyConstraint<ContactDetails, User>(c => c.User, u => u.ContactDetails),
                new ForeignKeyConstraint<Post, User, int>(p => p.Poster, p => p.PosterId, u => u.Id),
                new ForeignKeyConstraint<ContactDetails, User, int>(c => c.User, c => c.UserId, u => u.Id),
                new ManyToOnePropertyConstraint<CategoryLocalized, Category>(cl => cl.LocalizationFor, c => c.Localizations),
                new OneToManyPropertyConstraint<Category, CategoryLocalized>(c => c.Localizations, cl => cl.LocalizationFor),
                new ForeignKeyConstraint<CategoryLocalized, Category, int>(cl => cl.LocalizationFor, cl => cl.CategoryId, c => c.Id),
                new ManyToOnePropertyConstraint<PostLocalized, Post>(pl => pl.LocalizationFor, p => p.Localizations),
                new OneToManyPropertyConstraint<Post, PostLocalized>(p => p.Localizations, pl => pl.LocalizationFor),
                new ForeignKeyConstraint<PostLocalized, Post, int>(pl => pl.LocalizationFor, pl => pl.PostId, c => c.Id),
            };
            expected = expected.OrderBy(x => x.PropInfo.ToString()).ToArray();

            // act
            var metadata = GetMetadata();

            // assert
            var actual = metadata.Constraints.OrderBy(x => x.PropInfo.ToString());
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
