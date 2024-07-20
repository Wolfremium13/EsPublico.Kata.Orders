using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Domain.OrderItems;
using FluentAssertions;

namespace EsPublico.Kata.Orders.Tests.Domain.OrderItems;

public class DateShould
{
    [Fact]
    public void BeCreated()
    {
        const string aDate = "7/27/2012";
        var validDate = Date.Create(aDate);

        validDate.Match(
            value => value.ToString().Should().Be("27-07-2012"),
            error => error.Should().BeNull()
        );
    }

    [Fact]
    public void NotAllowInvalidDates()
    {
        var invalidDate = Date.Create("invalid-date");

        invalidDate.Match(
            value => value.Should().BeNull(),
            error => error.Should().BeOfType<InvalidParameter>()
                .Which.Message.Should().Be("Invalid Date")
        );
    }
}