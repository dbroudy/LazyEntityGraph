using FluentAssertions;
using LazyEntityGraph.Core;
using LazyEntityGraph.Core.Constraints;
using AutoFixture;
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
        public void SetsNullOnGeneratedProxy()
        {
            // arrange 
            var fixture = IntegrationTest.GetFixture(new ManyToOnePropertyConstraint<Foo, Bar>(f => f.Bar, b => b.Foos));
            var foo = fixture.Create<Foo>();

            // act
            foo.Bar = null;

            // assert
            foo.Bar.Should().BeNull();
        }

        [Fact]
        public void AddsDerivedItemToGeneratedCollection()
        {
            // arrange
            var fixture = IntegrationTest.GetFixture(new ManyToOnePropertyConstraint<Foo, Bar>(f => f.Bar, b => b.Foos));
            var foo = fixture.Create<Faz>();

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

        [Fact]
        public void RemovesItemFromCollection()
        {
            // arrange
            var fixture = IntegrationTest.GetFixture(new ManyToOnePropertyConstraint<Foo, Bar>(f => f.Bar, b => b.Foos));
            var foo = fixture.Create<Foo>();
            var bar = foo.Bar;

            // act
            foo.Bar = fixture.Create<Bar>();

            // assert
            bar.Foos.Should().NotContain(foo);
        }

        [Fact]
        public void ConstraintsAreEqualWhenPropertiesAreEqual()
        {
            // arrange
            var first = new ManyToOnePropertyConstraint<Foo, Bar>(f => f.Bar, b => b.Foos);
            var second = new ManyToOnePropertyConstraint<Foo, Bar>(x => x.Bar, x => x.Foos);

            // act and assert
            first.Should().Be(second);
        }
    }
}