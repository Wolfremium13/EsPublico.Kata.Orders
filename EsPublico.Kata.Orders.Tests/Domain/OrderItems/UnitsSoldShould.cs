using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Domain.OrderItems;
using FluentAssertions;

namespace EsPublico.Kata.Orders.Tests.Domain.OrderItems;

public class UnitsSoldShould
{
    [Fact]
    public void BeCreated()
    {
        const long aUnitsSold = 10;
        var validUnitsSold = UnitsSold.Create(aUnitsSold);

        validUnitsSold.Match(
            value => value.Value.Should().Be(aUnitsSold),
            error => error.Should().BeNull()
        );
    }

    [Fact]
    public void NotAllowNegativeValues()
    {
        var invalidUnitsSold = UnitsSold.Create(-1);

        invalidUnitsSold.Match(
            value => value.Should().BeNull(),
            error => error.Should().BeOfType<InvalidParameter>()
                .Which.Message.Should().Be("Units sold must be greater or equal to 0")
        );
    }
}