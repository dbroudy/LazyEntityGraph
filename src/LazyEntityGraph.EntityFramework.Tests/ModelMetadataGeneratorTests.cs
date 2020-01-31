using FluentAssertions;
using LazyEntityGraph.Core;
using LazyEntityGraph.Core.Constraints;
using LazyEntityGraph.EntityFramework;
using System.Collections;
using LazyEntityGraph.EntityFramework.Tests.Model;
using LazyEntityGraph.EntityFramework.Tests.Model.TPC;
using LazyEntityGraph.EntityFramework.Tests.Model.TPH;
using LazyEntityGraph.EntityFramework.Tests.Model.TPT;
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
                typeof (Post), typeof (User), typeof (Tag), typeof (ContactDetails), typeof (Category), typeof(Story),
                typeof (OneTimeInvoice), typeof (SubscriptionInvoice), typeof (PostLocalized), typeof (BankAccount),
                typeof (CreditCard), typeof (CategoryLocalized), typeof (BillingDetail), typeof (Invoice)
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
                new OneToManyPropertyConstraint<Category, CategoryLocalized>(c => c.Localizations, cl => cl.LocalizationFor),
                new OneToManyPropertyConstraint<Post, PostLocalized>(p => p.Localizations, pl => pl.LocalizationFor),
                new OneToManyPropertyConstraint<User, BillingDetail>(u => u.BillingDetails, bd => bd.User),
                new OneToManyPropertyConstraint<User, Invoice>(u => u.Invoices, i => i.User),
                new OneToManyPropertyConstraint<User, Post>(u => u.Posts, p => p.Poster),
                new OneToOnePropertyConstraint<ContactDetails, User>(c => c.User, u => u.ContactDetails),
                new OneToOnePropertyConstraint<User,ContactDetails>(u => u.ContactDetails, c => c.User),
                new ManyToManyPropertyConstraint<Post, Tag>(p => p.Tags, t => t.Posts),
                new ManyToManyPropertyConstraint<Tag, Post>(t => t.Posts, p => p.Tags),
                new ManyToOnePropertyConstraint<BillingDetail, User>(bd => bd.User, u => u.BillingDetails),
                new ManyToOnePropertyConstraint<CategoryLocalized, Category>(cl => cl.LocalizationFor, c => c.Localizations),
                new ManyToOnePropertyConstraint<Invoice, User>(i => i.User, u => u.Invoices),
                new ManyToOnePropertyConstraint<Post, User>(p => p.Poster, u => u.Posts),
                new ManyToOnePropertyConstraint<PostLocalized, Post>(pl => pl.LocalizationFor, p => p.Localizations),
                new ForeignKeyConstraint<BillingDetail, User, int>(bd => bd.User, bd => bd.UserId, u => u.Id),
                new ForeignKeyConstraint<CategoryLocalized, Category, int>(cl => cl.LocalizationFor, cl => cl.CategoryId, c => c.Id),
                new ForeignKeyConstraint<ContactDetails, User, int>(c => c.User, c => c.UserId, u => u.Id),
                new ForeignKeyConstraint<Invoice, User, int>(i => i.User, i => i.UserId, u => u.Id),
                new ForeignKeyConstraint<Post, User, int>(p => p.Poster, p => p.PosterId, u => u.Id),
                new ForeignKeyConstraint<PostLocalized, Post, int>(pl => pl.LocalizationFor, pl => pl.PostId, p => p.Id),
            };

            // act
            var metadata = GetMetadata();

            // assert
            metadata.Constraints.Should().BeEquivalentTo(expected);
        }
    }
}
