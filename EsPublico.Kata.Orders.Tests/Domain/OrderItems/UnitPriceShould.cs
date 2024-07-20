using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Domain.OrderItems;
using FluentAssertions;

namespace EsPublico.Kata.Orders.Tests.Domain.OrderItems;

public class UnitPriceShould
{
    [Fact]
    public void BeCreated()
    {
        const decimal aUnitPrice = 10;
        var validUnitPrice = UnitPrice.Create(aUnitPrice);

        validUnitPrice.Match(
            value => value.Value.Should().Be(aUnitPrice),
            error => error.Should().BeNull()
        );
    }

    [Fact]
    public void NotAllowNegativeValues()
    {
        var invalidUnitPrice = UnitPrice.Create(-1);

        invalidUnitPrice.Match(
            value => value.Should().BeNull(),
            error => error.Should().BeOfType<InvalidParameter>()
                .Which.Message.Should().Be("Unit price must be greater or equal to 0")
        );
    }

    [Fact]
    public void NotAllowMoreThanTwoDecimals()
    {
        var invalidUnitPrice = UnitPrice.Create(10.123m);

        invalidUnitPrice.Match(
            value => value.Should().BeNull(),
            error => error.Should().BeOfType<InvalidParameter>()
                .Which.Message.Should().Be("Unit price must contain at most 2 decimal places")
        );
    }
}