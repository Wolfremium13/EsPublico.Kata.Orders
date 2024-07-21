using LanguageExt;

namespace EsPublico.Kata.Orders.Domain.OrderItems;

public class Region
{
    private readonly string? _value;
    private Region(string? givenRegion) => _value = givenRegion;

    public static Either<Error, Region> Create(string? givenRegion)
    {
        if (string.IsNullOrWhiteSpace(givenRegion))
        {
            return new MissingParameter("Missing Region");
        }

        return new Region(givenRegion);
    }

    public override string? ToString() => _value;
}