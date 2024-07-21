using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Domain.OrderItems;
using FluentAssertions;

namespace EsPublico.Kata.Orders.Tests.Domain.OrderItems;

public class PriorityShould
{
    [Theory]
    [InlineData("C")]
    [InlineData("H")]
    [InlineData("M")]
    [InlineData("L")]
    public void BeCreatedForCertainValues(string? aValidPriority)
    {
        var validPriority = Priority.Create(aValidPriority);

        validPriority.Match(
            value => value.ToString().Should().Be(aValidPriority),
            error => error.Should().BeNull()
        );
    }

    [Fact]
    public void NotAllowInvalidValues()
    {
        var invalidPriority = Priority.Create("invalid-priority");

        invalidPriority.Match(
            value => value.Should().BeNull(),
            error => error.Should().BeOfType<InvalidParameter>()
                .Which.Message.Should().Be("Invalid Priority: invalid-priority, expected one of C, H, M, L")
        );
    }

    [Fact]
    public void NotAllowNulls()
    {
        var invalidPriority = Priority.Create(null!);

        invalidPriority.Match(
            value => value.Should().BeNull(),
            error => error.Should().BeOfType<MissingParameter>()
                .Which.Message.Should().Be("Missing Priority")
        );
    }
}