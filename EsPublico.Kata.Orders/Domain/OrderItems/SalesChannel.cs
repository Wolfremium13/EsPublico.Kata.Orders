using LanguageExt;

namespace EsPublico.Kata.Orders.Domain.OrderItems;

public class SalesChannel
{
    private readonly string? _value;
    private SalesChannel(string? givenSalesChannel) => _value = givenSalesChannel;

    public static Either<Error, SalesChannel> Create(string? givenSalesChannel)
    {
        if (string.IsNullOrWhiteSpace(givenSalesChannel))
        {
            return new MissingParameter("Missing SalesChannel");
        }

        return new SalesChannel(givenSalesChannel);
    }

    public override string? ToString() => _value;
}