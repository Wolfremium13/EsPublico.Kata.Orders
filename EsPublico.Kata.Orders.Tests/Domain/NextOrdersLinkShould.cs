using EsPublico.Kata.Orders.Domain;
using FluentAssertions;

namespace EsPublico.Kata.Orders.Tests.Domain;

public class NextOrdersLinkShould
{
    [Fact]
    public void BeCreated()
    {
        const string? aNextOrdersLink = "https://api.espublico.com/orders?page=2";
        var validNextOrdersLink = NextOrdersLink.Create(aNextOrdersLink);

        validNextOrdersLink.Match(
            value => value.Value.Should().Be(aNextOrdersLink),
            error => error.Should().BeNull()
        );
    }

    [Fact]
    public void NotAllowNulls()
    {
        var invalidNextOrdersLink = NextOrdersLink.Create(null!);

        invalidNextOrdersLink.Match(
            value => value.Should().BeNull(),
            error => error.Should().BeOfType<MissingParameter>()
                .Which.Message.Should().Be("Missing Next Orders Link")
        );
    }

    [Fact]
    public void NotAllowInvalidNextOrdersLinks()
    {
        var invalidNextOrdersLink = NextOrdersLink.Create("invalid-next-orders-link");

        invalidNextOrdersLink.Match(
            value => value.Should().BeNull(),
            error => error.Should().BeOfType<InvalidParameter>()
                .Which.Message.Should().Be("Invalid Next Orders Link")
        );
    }
}