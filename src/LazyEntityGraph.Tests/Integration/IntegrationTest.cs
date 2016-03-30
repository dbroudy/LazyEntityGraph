using FluentAssertions;
using LazyEntityGraph.AutoFixture;
using LazyEntityGraph.Core;
using Ploeh.AutoFixture;
using System.Collections.Generic;
using Xunit;

namespace LazyEntityGraph.Tests.Integration
{
    public class IntegrationTest
    {
        private Fixture GetFixture(params IPropertyConstraint[] constraints)
        {
            var entityTypes = new[] { typeof(Foo), typeof(Bar) };
            var customization = new LazyEntityGraphCustomization(entityTypes, constraints);
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

        [Fact]
        public void OneToOneConstraintSetsInversePropertyOnGeneratedProxy()
        {
            // arrange
            var fixture = GetFixture(new OneToOnePropertyConstraint<Foo, Bar>(f => f.Bar, b => b.Foo));
            var foo = fixture.Create<Foo>();

            // act
            var bar = foo.Bar;

            // assert
            bar.Foo.Should().BeSameAs(foo);
        }

        [Fact]
        public void OneToOneConstraintSetsInversePropertyOnExternalProxy()
        {
            // arrange
            var fixture = GetFixture(new OneToOnePropertyConstraint<Foo, Bar>(f => f.Bar, b => b.Foo));
            var foo = fixture.Create<Foo>();
            var bar = fixture.Create<Bar>();

            // act
            foo.Bar = bar;

            // assert
            bar.Foo.Should().BeSameAs(foo);
        }

        [Fact]
        public void OneToOneConstraintSetsInversePropertyOnPOCO()
        {
            // arrange
            var fixture = GetFixture(new OneToOnePropertyConstraint<Foo, Bar>(f => f.Bar, b => b.Foo));
            var foo = fixture.Create<Foo>();
            var bar = new Bar();

            // act
            foo.Bar = bar;

            bar.Foo.Should().BeSameAs(foo);
        }

        [Fact]
        public void OneToManyConstraintSetsInversePropertyOnGeneratedCollection()
        {
            // arrange
            var fixture = GetFixture(new OneToManyPropertyConstraint<Foo, Bar>(f => f.Bars, b => b.Foo));
            var foo = fixture.Create<Foo>();

            // act
            var bars = foo.Bars;

            // assert
            foreach (var bar in bars)
                bar.Foo.Should().BeSameAs(foo);
        }

        [Fact]
        public void OneToManyConstraintSetsInversePropertyOnExternalCollection()
        {
            // arrange
            var fixture = GetFixture(new OneToManyPropertyConstraint<Foo, Bar>(f => f.Bars, b => b.Foo));
            var foo = fixture.Create<Foo>();

            // act
            foo.Bars = fixture.Create<ICollection<Bar>>();

            // assert
            foreach (var bar in foo.Bars)
                bar.Foo.Should().BeSameAs(foo);
        }

        [Fact]
        public void OneToManyConstraintSetsInversePropertyOnPOCOAddedToGeneratedCollection()
        {
            // arrange
            var fixture = GetFixture(new OneToManyPropertyConstraint<Foo, Bar>(f => f.Bars, b => b.Foo));
            var foo = fixture.Create<Foo>();
            var bar = new Bar();

            // act
            foo.Bars.Add(bar);

            // assert
            bar.Foo.Should().BeSameAs(foo);
        }

        [Fact]
        public void OneToManyConstraintSetsInversePropertyOnProxyAddedToGeneratedCollection()
        {
            // arrange
            var fixture = GetFixture(new OneToManyPropertyConstraint<Foo, Bar>(f => f.Bars, b => b.Foo));
            var foo = fixture.Create<Foo>();
            var bar = fixture.Create<Bar>();

            // act
            foo.Bars.Add(bar);

            // assert
            bar.Foo.Should().BeSameAs(foo);
        }

        [Fact]
        public void ManyToOneConstraint()
        {
            // arrange
            var fixture = GetFixture(new ManyToOnePropertyConstraint<Foo, Bar>(f => f.Bar, b => b.Foos));
            var foo = fixture.Create<Foo>();

            // act
            var bar = foo.Bar;

            // assert
            bar.Foos.Should().Contain(foo);
        }
    }
}
