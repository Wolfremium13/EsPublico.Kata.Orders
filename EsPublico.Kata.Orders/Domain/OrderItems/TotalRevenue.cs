using LanguageExt;

namespace EsPublico.Kata.Orders.Domain.OrderItems;

public class TotalRevenue
{
    private TotalRevenue(decimal givenRevenue) => Value = givenRevenue;
    public decimal Value { get; }

    public static Either<Error, TotalRevenue> Create(decimal givenRevenue)
    {
        if (givenRevenue != decimal.Round(givenRevenue, 2))
        {
            return new InvalidParameter("Total revenue must contain at most 2 decimal places");
        }

        if (givenRevenue < 0)
        {
            return new InvalidParameter("Total revenue must be greater or equal to 0");
        }

        return new TotalRevenue(givenRevenue);
    }
}