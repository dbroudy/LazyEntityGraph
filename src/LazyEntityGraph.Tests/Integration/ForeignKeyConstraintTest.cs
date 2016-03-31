using FluentAssertions;
using LazyEntityGraph.Core.Constraints;
using Ploeh.AutoFixture;
using Xunit;

namespace LazyEntityGraph.Tests.Integration
{
    public class ForeignKeyConstraintTest
    {
        [Fact]
        public void SetsPropertyToGeneratedProxyProperty()
        {
            // arrange
            var fixture = IntegrationTest.GetFixture(new ForeignKeyConstraint<Foo, Bar, int>(f => f.Bar, f => f.BarId, b => b.Id));
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
            var fixture = IntegrationTest.GetFixture(new ForeignKeyConstraint<Foo, Bar, int>(f => f.Bar, f => f.BarId, b => b.Id));
            var foo = fixture.Create<Foo>();
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
            var first = new ForeignKeyConstraint<Foo, Bar, int>(f => f.Bar, f => f.BarId, b => b.Id);
            var second = new ForeignKeyConstraint<Foo, Bar, int>(x => x.Bar, x => x.BarId, x => x.Id);

            // act and assert
            first.Should().Be(second);
        }
    }
}
