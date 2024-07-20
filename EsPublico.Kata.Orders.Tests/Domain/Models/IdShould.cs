using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Domain.Models;
using FluentAssertions;

namespace EsPublico.Kata.Orders.Tests.Domain.Models;

public class IdShould
{
    [Fact]
    public void BeCreated()
    {
        var validId = Id.Create(1);

        validId.Match(
            value => value.Value.Should().Be(1),
            error => error.Should().BeNull()
        );
    }

    [Fact]
    public void NotAllowInvalidIds()
    {
        var invalidId = Id.Create(0);

        invalidId.Match(
            value => value.Should().BeNull(),
            error => error.Should().BeOfType<InvalidParameter>()
                .Which.Message.Should().Be("Invalid UUID")
        );
    }
}