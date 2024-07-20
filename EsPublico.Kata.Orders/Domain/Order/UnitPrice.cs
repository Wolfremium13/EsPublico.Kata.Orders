using LanguageExt;

namespace EsPublico.Kata.Orders.Domain.Order;

public class UnitPrice
{
    private UnitPrice(decimal givenPrice) => Value = givenPrice;
    public decimal Value { get; }

    public static Either<Error, UnitPrice> Create(decimal givenPrice)
    {
        if (givenPrice != decimal.Round(givenPrice, 2))
        {
            return new InvalidParameter("Unit price must contain at most 2 decimal places");
        }

        if (givenPrice < 0)
        {
            return new InvalidParameter("Unit price must be greater or equal to 0");
        }

        return new UnitPrice(givenPrice);
    }
}