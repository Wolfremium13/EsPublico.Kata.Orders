using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Domain.Order;
using FluentAssertions;

namespace EsPublico.Kata.Orders.Tests.Domain.Order;

public class TotalProfitShould
{
    [Fact]
    public void BeCreated()
    {
        const decimal aProfit = 10.5m;
        var validProfit = TotalProfit.Create(aProfit);

        validProfit.Match(
            value => value.Value.Should().Be(aProfit),
            error => error.Should().BeNull()
        );
    }

    [Fact]
    public void NotAllowMoreThanTwoDecimals()
    {
        var invalidProfit = TotalProfit.Create(10.123m);

        invalidProfit.Match(
            value => value.Should().BeNull(),
            error => error.Should().BeOfType<InvalidParameter>()
                .Which.Message.Should().Be("Total profit must contain at most 2 decimal places")
        );
    }
}