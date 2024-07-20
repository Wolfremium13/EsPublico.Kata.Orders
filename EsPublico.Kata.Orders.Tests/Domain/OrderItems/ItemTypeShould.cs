using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Domain.OrderItems;
using FluentAssertions;

namespace EsPublico.Kata.Orders.Tests.Domain.OrderItems;

public class ItemTypeShould
{
    [Fact]
    public void BeCreated()
    {
        const string anItemType = "a-type";
        var validItemType = ItemType.Create(anItemType);

        validItemType.Match(
            value => value.ToString().Should().Be(anItemType),
            error => error.Should().BeNull()
        );
    }

    [Fact]
    public void NotAllowNulls()
    {
        var invalidItemType = ItemType.Create(null);

        invalidItemType.Match(
            value => value.Should().BeNull(),
            error => error.Should().BeOfType<MissingParameter>()
                .Which.Message.Should().Be("Missing ItemType")
        );
    }
}