using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Domain.Models;
using FluentAssertions;

namespace EsPublico.Kata.Orders.Tests.Domain.Models;

public class UuidShould
{
    [Fact]
    public void BeCreated()
    {
        const string validUuid = "123e4567-e89b-12d3-a456-426614174000";

        var uuid = Uuid.Create(validUuid);

        uuid.Match(
            value => value.ToString().Should().Be(validUuid),
            error => error.Should().BeNull()
        );
    }

    [Fact]
    public void NotAllowInvalidUuids()
    {
        const string invalidUuid = "invalid-uuid";

        var uuid = Uuid.Create(invalidUuid);

        uuid.Match(
            value => value.Should().BeNull(),
            error => error.Should().BeOfType<InvalidParameter>()
                .Which.Message.Should().Be("Invalid UUID")
        );
    }
}