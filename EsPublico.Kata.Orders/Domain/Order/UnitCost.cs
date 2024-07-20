using LanguageExt;

namespace EsPublico.Kata.Orders.Domain.Order;

public class UnitCost
{
    private UnitCost(decimal givenCost) => Value = givenCost;
    public decimal Value { get; }

    public static Either<Error, UnitCost> Create(decimal givenCost)
    {
        if (givenCost != decimal.Round(givenCost, 2))
        {
            return new InvalidParameter("Unit price must contain at most 2 decimal places");
        }

        if (givenCost < 0)
        {
            return new InvalidParameter("Unit cost must be greater or equal to 0");
        }

        return new UnitCost(givenCost);
    }
}