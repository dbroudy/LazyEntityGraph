using FluentAssertions;
using LazyEntityGraph.Core;
using LazyEntityGraph.Core.Constraints;
using Ploeh.AutoFixture;
using Xunit;

namespace LazyEntityGraph.Tests.Integration
{
    public class OneToOneConstraintTest
    {
        [Fact]
        public void SetsInversePropertyOnGeneratedProxy()
        {
            // arrange
            var fixture = IntegrationTest.GetFixture(new OneToOnePropertyConstraint<Foo, Bar>(f => f.Bar, b => b.Foo));
            var foo = fixture.Create<Foo>();

            // act
            var bar = foo.Bar;

            // assert
            bar.Foo.Should().BeSameAs(foo);
        }

        [Fact]
        public void SetsInversePropertyOnExternalProxy()
        {
            // arrange
            var fixture = IntegrationTest.GetFixture(new OneToOnePropertyConstraint<Foo, Bar>(f => f.Bar, b => b.Foo));
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
            var fixture = IntegrationTest.GetFixture(new OneToOnePropertyConstraint<Foo, Bar>(f => f.Bar, b => b.Foo));
            var foo = fixture.Create<Foo>();
            var bar = new Bar();

            // act
            foo.Bar = bar;

            bar.Foo.Should().BeSameAs(foo);
        }
    }
}