using LanguageExt;

namespace EsPublico.Kata.Orders.Domain.OrderItems;

public class Region
{
    private Region(string givenRegion) => Value = givenRegion;
    
    public string Value { get; }

    public static Either<Error, Region> Create(string? givenRegion)
    {
        if (string.IsNullOrWhiteSpace(givenRegion))
        {
            return new MissingParameter("Missing Region");
        }

        return new Region(givenRegion);
    }
}