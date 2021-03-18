using FluentAssertions;
using LazyEntityGraph.AutoFixture;
using LazyEntityGraph.Core;
using LazyEntityGraph.Core.Constraints;
using AutoFixture;
using System.Collections.Generic;
using Xunit;

namespace LazyEntityGraph.Tests.Integration
{
    public class IntegrationTest
    {
        public static Fixture GetFixture(params IPropertyConstraint[] constraints)
        {
            var customization = new LazyEntityGraphCustomization(
                new ModelMetadata(
                    new[] { typeof(Foo), typeof(Bar), typeof(Faz), typeof(Baz) },
                    constraints));

            var fixture = new Fixture();
            fixture.Customize(customization);
            return fixture;
        }

        [Fact]
        public void PropertyGetterCreatesObject()
        {
            // arrange
            var fixture = GetFixture();
            var foo = fixture.Create<Foo>();

            // act
            var bar = foo.Bar;

            // assert
            bar.Should().NotBeNull();
        }

        [Fact]
        public void PropertyShouldStayNullWhenSetToNull()
        {
            // arrange
            var fixture = GetFixture();
            var foo = fixture.Create<Foo>();

            // act
            foo.Bar = null;

            // assert
            foo.Bar.Should().BeNull();
        }

        [Fact]
        public void PropertyShouldStaySameWhenSet()
        {
            // arrange
            var fixture = GetFixture();
            var foo = fixture.Create<Foo>();
            var bar = new Bar();

            // act
            foo.Bar = bar;

            // assert
            foo.Bar.Should().BeSameAs(bar);
        }

        [Fact]
        public void CollectionPropertyGeneratesCollection()
        {
            // arrange
            var fixture = GetFixture();
            var foo = fixture.Create<Foo>();

            // act
            var bars = foo.Bars;

            // assert
            bars.Should().NotBeEmpty();
        }

        [Fact]
        public void CollectionPropertyCanBeSet()
        {
            // arrange
            var fixture = GetFixture();
            var foo = fixture.Create<Foo>();
            var bars = fixture.Create<ICollection<Bar>>();

            // act
            foo.Bars = bars;

            // assert
            foo.Bars.Should().BeEquivalentTo(bars);
        }
    }
}
