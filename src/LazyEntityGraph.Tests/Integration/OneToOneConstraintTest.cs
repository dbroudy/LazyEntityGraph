using FluentAssertions;
using LazyEntityGraph.Core;
using LazyEntityGraph.Core.Constraints;
using AutoFixture;
using Xunit;
using LazyEntityGraph.TestUtils;

namespace LazyEntityGraph.Tests.Integration
{
    public class OneToOneConstraintTest
    {
        [Fact]
        public void SetsInversePropertyOnGeneratedProxy()
        {
            // arrange
            var fixture = IntegrationTest.GetFixture(ExpectedConstraints.CreateOneToOne<Foo, Bar>(f => f.Bar, b => b.Foo));
            var foo = fixture.Create<Foo>();

            // act
            var bar = foo.Bar;

            // assert
            bar.Foo.Should().BeSameAs(foo);
        }

        [Fact]
        public void SetsNullOnGeneratedProxy()
        {
            // arrange
            var fixture = IntegrationTest.GetFixture(ExpectedConstraints.CreateOneToOne<Foo, Bar>(f => f.Bar, b => b.Foo));
            var foo = fixture.Create<Foo>();
            var bar = foo.Bar;

            // act
            foo.Bar = null;

            // assert
            foo.Bar.Should().BeNull();
        }

        [Fact]
        public void SetsInversePropertyOnDerivedGeneratedProxy()
        {
            // arrange
            var fixture = IntegrationTest.GetFixture(ExpectedConstraints.CreateOneToOne<Foo, Bar>(f => f.Bar, b => b.Foo));
            var foo = fixture.Create<Faz>();

            // act
            var bar = foo.Bar;

            // assert
            bar.Foo.Should().BeSameAs(foo);
        }

        [Fact]
        public void SetsInversePropertyOnExternalProxy()
        {
            // arrange
            var fixture = IntegrationTest.GetFixture(ExpectedConstraints.CreateOneToOne<Foo, Bar>(f => f.Bar, b => b.Foo));
            var foo = fixture.Create<Foo>();
            var bar = fixture.Create<Bar>();

            // act
            foo.Bar = bar;

            // assert
            bar.Foo.Should().BeSameAs(foo);
        }

        [Fact]
        public void SetsInversePropertyOnPOCO()
        {
            // arrange
            var fixture = IntegrationTest.GetFixture(ExpectedConstraints.CreateOneToOne<Foo, Bar>(f => f.Bar, b => b.Foo));
            var foo = fixture.Create<Foo>();
            var bar = new Bar();

            // act
            foo.Bar = bar;

            // assert
            bar.Foo.Should().BeSameAs(foo);
        }

        [Fact]
        public void ConstraintsAreEqualWhenPropertiesAreEqual()
        {
            // arrange
            var first = ExpectedConstraints.CreateOneToOne<Foo, Bar>(f => f.Bar, b => b.Foo);
            var second = ExpectedConstraints.CreateOneToOne<Foo, Bar>(x => x.Bar, x => x.Foo);

            // act and assert
            first.Should().Be(second);
        }
    }
}