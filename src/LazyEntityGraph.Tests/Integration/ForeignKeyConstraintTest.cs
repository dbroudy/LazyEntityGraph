using FluentAssertions;
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
            var faz = fixture.Create<Faz>();

            // act
            var bar = faz.Bar;

            // assert
            faz.BarId.Should().Be(bar.Id);
        }

        [Fact]
        public void SetsDerivedPropertyToPOCOProperty()
        {
            // arrange
            var fixture = IntegrationTest.GetFixture(ExpectedConstraints.CreateForeignKey<Foo, Bar, int>(f => f.Bar, f => f.BarId, b => b.Id));
            var faz = fixture.Create<Faz>();
            var bar = new Bar() { Id = fixture.Create<int>() };

            // act
            faz.Bar = bar;

            // assert
            faz.BarId.Should().Be(bar.Id);
        }

        [Fact]
        public void SetsOneSidedPropertyToGeneratedProxyProperty()
        {
            // arrange
            var fixture = IntegrationTest.GetFixture(ExpectedConstraints.CreateForeignKey<Baz, Foo, int>(b => b.Foo, b => b.FooId, f => f.Id));
            var baz = fixture.Create<Baz>();

            // act
            var foo = baz.Foo;

            // assert
            baz.FooId.Should().Be(foo.Id);
        }
        [Fact]
        public void SetsOneSidedPropertyToPOCOProperty()
        {
            // arrange
            var fixture = IntegrationTest.GetFixture(ExpectedConstraints.CreateForeignKey<Baz, Foo, int>(b => b.Foo, b => b.FooId, f => f.Id));
            var baz = fixture.Create<Baz>();
            var foo = new Foo() { Id = fixture.Create<int>() };

            // act
            baz.Foo = foo;

            // assert
            baz.FooId.Should().Be(foo.Id);
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

        [Fact]
        public void MultiSetsPropertyToGeneratedProxyProperty()
        {
            // arrange
            var fixture = IntegrationTest.GetFixture(ExpectedConstraints.CreateForeignKey<MultiFoo, MultiBar, long, string>(
                f => f.Bar,
                (f => f.BarId1, f => f.BarId2),
                (b => b.Id1, b => b.Id2)));
            var foo = fixture.Create<MultiFoo>();

            // act
            var bar = foo.Bar;

            // assert
            foo.BarId1.Should().Be(bar.Id1);
            foo.BarId2.Should().Be(bar.Id2);
        }

        [Fact]
        public void MultiSetsPropertyToPOCOProperty()
        {
            // arrange
            var fixture = IntegrationTest.GetFixture(ExpectedConstraints.CreateForeignKey<MultiFoo, MultiBar, long, string>(
                f => f.Bar,
                ( f => f.BarId1, f => f.BarId2 ),
                ( b => b.Id1, b => b.Id2 )));
            var foo = fixture.Create<MultiFoo>();
            var bar = new MultiBar() { Id1 = fixture.Create<long>(), Id2 = fixture.Create<string>() };

            // act
            foo.Bar = bar;

            // assert
            foo.BarId1.Should().Be(bar.Id1);
            foo.BarId2.Should().Be(bar.Id2);
        }

        [Fact]
        public void MultiSetsPropertyToNull()
        {
            // arrange
            var fixture = IntegrationTest.GetFixture(ExpectedConstraints.CreateForeignKey<MultiFoo, MultiBar, long, string>(
                f => f.Bar,
                ( f => f.BarId1, f => f.BarId2 ),
                ( b => b.Id1, b => b.Id2 )));
            var foo = fixture.Create<MultiFoo>();
            MultiBar bar = null;

            // act
            foo.Bar = bar;

            // assert
            foo.BarId1.Should().Be(0);
            foo.BarId2.Should().Be(null);
        }
    }
}
