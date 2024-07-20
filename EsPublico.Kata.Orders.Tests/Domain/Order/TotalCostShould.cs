using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Domain.Order;
using FluentAssertions;

namespace EsPublico.Kata.Orders.Tests.Domain.Order;

public class TotalCostShould
{
    [Fact]
    public void BeCreated()
    {
        const decimal aCost = 10.5m;
        var validCost = TotalCost.Create(aCost);

        validCost.Match(
            value => value.Value.Should().Be(aCost),
            error => error.Should().BeNull()
        );
    }

    [Fact]
    public void NotAllowNegativeValues()
    {
        var invalidCost = TotalCost.Create(-1);

        invalidCost.Match(
            value => value.Should().BeNull(),
            error => error.Should().BeOfType<InvalidParameter>()
                .Which.Message.Should().Be("Total cost must be greater or equal to 0")
        );
    }

    [Fact]
    public void NotAllowMoreThanTwoDecimals()
    {
        var invalidCost = TotalCost.Create(10.123m);

        invalidCost.Match(
            value => value.Should().BeNull(),
            error => error.Should().BeOfType<InvalidParameter>()
                .Which.Message.Should().Be("Total cost must contain at most 2 decimal places")
        );
    }
}