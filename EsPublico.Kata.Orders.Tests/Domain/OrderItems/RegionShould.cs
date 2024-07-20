using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Domain.OrderItems;
using FluentAssertions;

namespace EsPublico.Kata.Orders.Tests.Domain.OrderItems;

public class RegionShould
{
    [Fact]
    public void BeCreated()
    {
        const string aRegion = "Europe";
        var validRegion = Region.Create(aRegion);

        validRegion.Match(
            value => value.ToString().Should().Be(aRegion),
            error => error.Should().BeNull()
        );
    }

    [Fact]
    public void NotAllowNulls()
    {
        var invalidRegion = Region.Create(null!);

        invalidRegion.Match(
            value => value.Should().BeNull(),
            error => error.Should().BeOfType<MissingParameter>()
                .Which.Message.Should().Be("Missing Region")
        );
    }
}