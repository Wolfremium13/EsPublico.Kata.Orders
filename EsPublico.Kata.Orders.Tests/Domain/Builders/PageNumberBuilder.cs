using EsPublico.Kata.Orders.Domain;

namespace EsPublico.Kata.Orders.Tests.Domain.Builders;

public class PageNumberBuilder
{
    private int _value;

    public PageNumberBuilder WithValue(int value)
    {
        _value = value;
        return this;
    }

    public PageNumber Build() => PageNumber.Create(_value).Match(
        Right: pageNumber => pageNumber,
        Left: error => throw new ArgumentException(error.Message)
    );
}