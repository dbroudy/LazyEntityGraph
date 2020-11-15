using FluentAssertions;
using LazyEntityGraph.Core.Constraints;
using AutoFixture;
using Xunit;
using LazyEntityGraph.TestUtils;

namespace LazyEntityGraph.Tests.Integration
{
    public class ForeignKeyConstraintTest
    {
        [Fact]
        public void SetsPropertyToGeneratedProxyProperty()
        {
            // arrange
            var fixture = IntegrationTest.GetFixture(ExpectedConstraints.CreateForeignKey<Foo, Bar, int>(f => f.Bar, f => f.BarId, b => b.Id));
            var foo = fixture.Create<Foo>();

            // act
            var bar = foo.Bar;

            // assert
            foo.BarId.Should().Be(bar.Id);
        }
        [Fact]
        public void SetsPropertyToPOCOProperty()
        {
            // arrange
            var fixture = IntegrationTest.GetFixture(ExpectedConstraints.CreateForeignKey<Foo, Bar, int>(f => f.Bar, f => f.BarId, b => b.Id));
            var foo = fixture.Create<Foo>();
            var bar = new Bar() { Id = fixture.Create<int>() };

            // act
            foo.Bar = bar;

            // assert
            foo.BarId.Should().Be(bar.Id);
        }
        [Fact]
        public void SetsPropertyToNull()
        {
            // arrange
            var fixture = IntegrationTest.GetFixture(ExpectedConstraints.CreateForeignKey<Foo, Bar, int>(f => f.Bar, f => f.BarId, b => b.Id));
            var foo = fixture.Create<Foo>();
            Bar bar = null;

            // act
            foo.Bar = bar;

            // assert
            foo.BarId.Should().Be(0);
        }

        [Fact]
        public void SetsDerivedPropertyToGeneratedProxyProperty()
        {
            // arrange
            var fixture = IntegrationTest.GetFixture(ExpectedConstraints.CreateForeignKey<Foo, Bar, int>(f => f.Bar, f => f.BarId, b => b.Id));
            var foo = fixture.Create<Faz>();

            // act
            var bar = foo.Bar;

            // assert
            foo.BarId.Should().Be(bar.Id);
        }
        [Fact]
        public void SetsDerivedPropertyToPOCOProperty()
        {
            // arrange
            var fixture = IntegrationTest.GetFixture(ExpectedConstraints.CreateForeignKey<Foo, Bar, int>(f => f.Bar, f => f.BarId, b => b.Id));
            var foo = fixture.Create<Faz>();
            var bar = new Bar() { Id = fixture.Create<int>() };

            // act
            foo.Bar = bar;

            // assert
            foo.BarId.Should().Be(bar.Id);
        }

        [Fact]
        public void ConstraintsAreEqualWhenPropertiesAreEqual()
        {
            // arrange
            var first = ExpectedConstraints.CreateForeignKey<Foo, Bar, int>(f => f.Bar, f => f.BarId, b => b.Id);
            var second = ExpectedConstraints.CreateForeignKey<Foo, Bar, int>(x => x.Bar, x => x.BarId, x => x.Id);

            // act and assert
            first.Should().Be(second);
            first.GetHashCode().Should().Be(second.GetHashCode());
        }
    }
}
