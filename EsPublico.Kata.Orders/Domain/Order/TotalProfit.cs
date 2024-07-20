using LanguageExt;

namespace EsPublico.Kata.Orders.Domain.Order;

public class TotalProfit
{
    private TotalProfit(decimal givenProfit) => Value = givenProfit;
    public decimal Value { get; }

    public static Either<Error, TotalProfit> Create(decimal givenProfit)
    {
        if (givenProfit != decimal.Round(givenProfit, 2))
        {
            return new InvalidParameter("Total profit must contain at most 2 decimal places");
        }

        return new TotalProfit(givenProfit);
    }
}