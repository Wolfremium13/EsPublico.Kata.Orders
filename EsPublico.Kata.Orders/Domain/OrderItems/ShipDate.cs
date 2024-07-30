using LanguageExt;

namespace EsPublico.Kata.Orders.Domain.OrderItems;

public class ShipDate
{
    private ShipDate(DateTime givenDate) => Value = givenDate;
    public DateTime Value { get; }

    public static Either<Error, ShipDate> Create(string? givenDate)
    {
        if (!DateTime.TryParse(givenDate, out var date))
        {
            return new InvalidParameter("Invalid Date");
        }

        return new ShipDate(date);
    }

    public override string ToString() => Value.ToString("dd-MM-yyyy");
}