using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Domain.OrderItems;
using FluentAssertions;

namespace EsPublico.Kata.Orders.Tests.Domain.OrderItems;

public class UuidShould
{
    [Fact]
    public void BeCreated()
    {
        const string anUuid = "123e4567-e89b-12d3-a456-426614174000";
        var validUuid = Uuid.Create(anUuid);

        validUuid.Match(
            value => value.ToString().Should().Be(anUuid),
            error => error.Should().BeNull()
        );
    }

    [Fact]
    public void NotAllowNulls()
    {
        var invalidUuid = Uuid.Create(null!);

        invalidUuid.Match(
            value => value.Should().BeNull(),
            error => error.Should().BeOfType<MissingParameter>()
                .Which.Message.Should().Be("Missing UUID")
        );
    }

    [Fact]
    public void NotAllowInvalidUuids()
    {
        var invalidUuid = Uuid.Create("invalid-uuid");

        invalidUuid.Match(
            value => value.Should().BeNull(),
            error => error.Should().BeOfType<InvalidParameter>()
                .Which.Message.Should().Be("Invalid UUID")
        );
    }
}