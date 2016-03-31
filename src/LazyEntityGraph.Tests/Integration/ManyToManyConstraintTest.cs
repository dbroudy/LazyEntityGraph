using FluentAssertions;
using LazyEntityGraph.Core;
using Ploeh.AutoFixture;
using Xunit;

namespace LazyEntityGraph.Tests.Integration
{
    public class ManyToManyConstraintTest
    {
        [Fact]
        public void AddsItemToGeneratedCollection()
        {
            // arrange
            var fixture = IntegrationTest.GetFixture(new ManyToManyPropertyConstraint<Foo, Bar>(f => f.Bars, b => b.Foos));
            var foo = fixture.Create<Foo>();

            // act
            var bars = foo.Bars;

            // assert
            foreach (var bar in bars)
                bar.Foos.Should().Contain(foo);
        }
    }
}