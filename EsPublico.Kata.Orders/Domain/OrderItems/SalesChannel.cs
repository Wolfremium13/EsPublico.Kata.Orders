using LanguageExt;

namespace EsPublico.Kata.Orders.Domain.OrderItems;

public class SalesChannel
{
    private SalesChannel(string givenSalesChannel) => Value = givenSalesChannel;
    public string Value { get; }

    public static Either<Error, SalesChannel> Create(string? givenSalesChannel)
    {
        if (string.IsNullOrWhiteSpace(givenSalesChannel))
        {
            return new MissingParameter("Missing SalesChannel");
        }

        return new SalesChannel(givenSalesChannel);
    }
}