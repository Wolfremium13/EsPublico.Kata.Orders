using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Domain.Order;
using FluentAssertions;

namespace EsPublico.Kata.Orders.Tests.Domain.Order;

public class TotalRevenueShould
{
    [Fact]
    public void BeCreated()
    {
        const decimal aRevenue = 10.5m;
        var validRevenue = TotalRevenue.Create(aRevenue);

        validRevenue.Match(
            value => value.Value.Should().Be(aRevenue),
            error => error.Should().BeNull()
        );
    }

    [Fact]
    public void NotAllowNegativeValues()
    {
        var invalidRevenue = TotalRevenue.Create(-1);

        invalidRevenue.Match(
            value => value.Should().BeNull(),
            error => error.Should().BeOfType<InvalidParameter>()
                .Which.Message.Should().Be("Total revenue must be greater or equal to 0")
        );
    }

    [Fact]
    public void NotAllowMoreThanTwoDecimals()
    {
        var invalidRevenue = TotalRevenue.Create(10.123m);

        invalidRevenue.Match(
            value => value.Should().BeNull(),
            error => error.Should().BeOfType<InvalidParameter>()
                .Which.Message.Should().Be("Total revenue must contain at most 2 decimal places")
        );
    }
}