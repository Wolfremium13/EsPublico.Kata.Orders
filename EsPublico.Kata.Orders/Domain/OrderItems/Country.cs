using LanguageExt;

namespace EsPublico.Kata.Orders.Domain.OrderItems;

public class Country
{
    public string Value { get; }
    private Country(string givenCountry) => Value = givenCountry;

    public static Either<Error, Country> Create(string? givenCountry)
    {
        if (string.IsNullOrWhiteSpace(givenCountry))
        {
            return new MissingParameter("Missing Country");
        }

        return new Country(givenCountry);
    }

}