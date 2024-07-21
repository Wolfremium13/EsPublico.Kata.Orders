using EsPublico.Kata.Orders.Domain;
using FluentAssertions;

namespace EsPublico.Kata.Orders.Tests.Domain;

public class PageNumberShould
{
    [Fact]
    public void BeCreated()
    {
        const int aPageNumber = 1;
        var validPageNumber = PageNumber.Create(aPageNumber);

        validPageNumber.Match(
            value => value.ToString().Should().Be(aPageNumber.ToString()),
            error => error.Should().BeNull()
        );
    }

    [Fact]
    public void NotAllowInvalidPageNumbers()
    {
        var invalidPageNumber = PageNumber.Create(0);

        invalidPageNumber.Match(
            value => value.Should().BeNull(),
            error => error.Should().BeOfType<InvalidParameter>()
                .Which.Message.Should().Be("Invalid Page Number")
        );
    }
}