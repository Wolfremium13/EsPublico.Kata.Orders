using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Domain.Order;
using FluentAssertions;

namespace EsPublico.Kata.Orders.Tests.Domain.Order;

public class SalesChannelShould
{
    [Fact]
    public void BeCreated()
    {
        const string aSalesChannel = "a-sales-channel";
        var validSalesChannel = SalesChannel.Create(aSalesChannel);

        validSalesChannel.Match(
            value => value.ToString().Should().Be(aSalesChannel),
            error => error.Should().BeNull()
        );
    }

    [Fact]
    public void NotAllowNulls()
    {
        var invalidSalesChannel = SalesChannel.Create(null!);

        invalidSalesChannel.Match(
            value => value.Should().BeNull(),
            error => error.Should().BeOfType<MissingParameter>()
                .Which.Message.Should().Be("Missing SalesChannel")
        );
    }
}