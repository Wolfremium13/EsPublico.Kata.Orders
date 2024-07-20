using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Domain.Order;
using FluentAssertions;

namespace EsPublico.Kata.Orders.Tests.Domain.Order;

public class UnitCostShould
{
    [Fact]
    public void BeCreated()
    {
        const decimal aUnitCost = 10;
        var validUnitCost = UnitCost.Create(aUnitCost);

        validUnitCost.Match(
            value => value.Value.Should().Be(aUnitCost),
            error => error.Should().BeNull()
        );
    }

    [Fact]
    public void NotAllowNegativeValues()
    {
        var invalidUnitCost = UnitCost.Create(-1);

        invalidUnitCost.Match(
            value => value.Should().BeNull(),
            error => error.Should().BeOfType<InvalidParameter>()
                .Which.Message.Should().Be("Unit cost must be greater or equal to 0")
        );
    }

    [Fact]
    public void NotAllowMoreThanTwoDecimalPlaces()
    {
        var invalidUnitCost = UnitCost.Create(10.123m);

        invalidUnitCost.Match(
            value => value.Should().BeNull(),
            error => error.Should().BeOfType<InvalidParameter>()
                .Which.Message.Should().Be("Unit price must contain at most 2 decimal places")
        );
    }
}