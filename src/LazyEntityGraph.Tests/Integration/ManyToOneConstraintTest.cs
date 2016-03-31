using FluentAssertions;
using LazyEntityGraph.Core;
using Ploeh.AutoFixture;
using Xunit;

namespace LazyEntityGraph.Tests.Integration
{
    public class ManyToOneConstraintTest
    {
        [Fact]
        public void AddsItemToGeneratedCollection()
        {
            // arrange
            var fixture = IntegrationTest.GetFixture(new ManyToOnePropertyConstraint<Foo, Bar>(f => f.Bar, b => b.Foos));
            var foo = fixture.Create<Foo>();

            // act
            var bar = foo.Bar;

            // assert
            bar.Foos.Should().Contain(foo);
        }

        [Fact]
        public void AddsItemToPOCOCollection()
        {
            // arrange 
            var fixture = IntegrationTest.GetFixture(new ManyToOnePropertyConstraint<Foo, Bar>(f => f.Bar, b => b.Foos));
            var foo = fixture.Create<Foo>();
            var bar = new Bar();

            // act
            foo.Bar = bar;

            // assert
            bar.Foos.Should().Contain(foo);
        }
    }
}