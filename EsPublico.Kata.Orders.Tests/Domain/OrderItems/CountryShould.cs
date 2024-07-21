using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Domain.OrderItems;
using FluentAssertions;

namespace EsPublico.Kata.Orders.Tests.Domain.OrderItems;

public class CountryShould
{
    [Fact]
    public void BeCreated()
    {
        const string? aCountry = "Spain";
        var validCountry = Country.Create(aCountry);

        validCountry.Match(
            value => value.ToString().Should().Be(aCountry),
            error => error.Should().BeNull()
        );
    }

    [Fact]
    public void NotAllowNulls()
    {
        var invalidCountry = Country.Create(null!);

        invalidCountry.Match(
            value => value.Should().BeNull(),
            error => error.Should().BeOfType<MissingParameter>()
                .Which.Message.Should().Be("Missing Country")
        );
    }
}