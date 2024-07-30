using LanguageExt;

namespace EsPublico.Kata.Orders.Domain.OrderItems;

public class Date
{
    private Date(DateTime givenDate) => Value = givenDate;
    public DateTime Value { get; }

    public static Either<Error, Date> Create(string? givenDate)
    {
        if (!DateTime.TryParse(givenDate, out var date))
        {
            return new InvalidParameter("Invalid Date");
        }

        return new Date(date);
    }

    public override string ToString() => Value.ToString("dd-MM-yyyy");
}