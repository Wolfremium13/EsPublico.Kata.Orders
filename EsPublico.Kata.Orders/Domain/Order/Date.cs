using LanguageExt;

namespace EsPublico.Kata.Orders.Domain.Order;

public class Date
{
    private readonly DateTime _value;
    private Date(DateTime givenDate) => _value = givenDate;

    public static Either<Error, Date> Create(string givenDate)
    {
        if (!DateTime.TryParse(givenDate, out var date))
        {
            return new InvalidParameter("Invalid Date");
        }

        return new Date(date);
    }

    public override string ToString() => _value.ToString("dd-MM-yyyy");
}