using LanguageExt;

namespace EsPublico.Kata.Orders.Domain.OrderItems;

public class UnitsSold
{
    private UnitsSold(long givenUnits) => Value = givenUnits;
    public long Value { get; }

    public static Either<Error, UnitsSold> Create(long givenUnits)
    {
        if (givenUnits < 0)
        {
            return new InvalidParameter("Units sold must be greater or equal to 0");
        }

        return new UnitsSold(givenUnits);
    }
}