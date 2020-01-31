using FluentAssertions;
using Xunit;
using LazyEntityGraph.EntityFramework.Tests.Model;
using LazyEntityGraph.EntityFramework.Tests.Model.TPC;
using LazyEntityGraph.EntityFramework.Tests.Model.TPH;
using LazyEntityGraph.EntityFramework.Tests.Model.TPT;

// ReSharper disable InconsistentNaming

namespace LazyEntityGraph.EntityFramework.Tests
{
    public class EndToEndTests
    {
        private readonly BlogContext _context;

        public EndToEndTests()
        {
            _context = new BlogContext();
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
        public void TPHBaseCanBeCreated(Post post)
        {
            var unused = post;
        }

        [Theory, BlogModelData]
        public void TPHDerivedCanBeCreated(Story story)
        {
            var unused = story;
        }

        [Theory, BlogModelData]
        public void ForeignKeyPropertyOnTPCOneToMany(SubscriptionInvoice subscriptionInvoice)
        {
            // assert
            subscriptionInvoice.User.Id.Should().Be(subscriptionInvoice.UserId);
        }

        [Theory, BlogModelData]
        public void ForeignKeyPropertyOnTPCManyToOne(User user)
        {
            // assert
            foreach (var invoice in user.Invoices)
                invoice.UserId.Should().Be(user.Id);
        }

        [Theory, BlogModelData]
        public void ForeignKeyPropertyOnTPTOneToMany(CreditCard creditCard)
        {
            // assert
            creditCard.User.Id.Should().Be(creditCard.UserId);
        }

        [Theory, BlogModelData]
        public void ForeignKeyPropertyOnTPTManyToOne(User user)
        {
            // assert
            foreach (var billingDetail in user.BillingDetails)
                billingDetail.UserId.Should().Be(user.Id);
        }
    }
}
