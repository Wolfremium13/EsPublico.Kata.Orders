using LanguageExt;

namespace EsPublico.Kata.Orders.Domain.OrderItems;

public class TotalCost
{
    private TotalCost(decimal givenCost) => Value = givenCost;
    public decimal Value { get; }

    public static Either<Error, TotalCost> Create(decimal givenCost)
    {
        if (givenCost != decimal.Round(givenCost, 2))
        {
            return new InvalidParameter("Total cost must contain at most 2 decimal places");
        }

        if (givenCost < 0)
        {
            return new InvalidParameter("Total cost must be greater or equal to 0");
        }

        return new TotalCost(givenCost);
    }
}