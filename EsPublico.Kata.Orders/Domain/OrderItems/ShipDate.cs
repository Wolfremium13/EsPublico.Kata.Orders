using LanguageExt;

namespace EsPublico.Kata.Orders.Domain.Order;

public class ShipDate
{
    private readonly DateTime _value;
    private ShipDate(DateTime givenDate) => _value = givenDate;

    public static Either<Error, ShipDate> Create(string? givenDate)
    {
        if (!DateTime.TryParse(givenDate, out var date))
        {
            return new InvalidParameter("Invalid Date");
        }

        return new ShipDate(date);
    }

    public override string ToString() => _value.ToString("dd-MM-yyyy");
}