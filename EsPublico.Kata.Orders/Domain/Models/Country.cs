using LanguageExt;

namespace EsPublico.Kata.Orders.Domain.Models;

public class Country
{
    private readonly string _value;
    private Country(string givenCountry) => _value = givenCountry;

    public static Either<Error, Country> Create(string givenCountry)
    {
        if (string.IsNullOrWhiteSpace(givenCountry))
        {
            return new MissingParameter("Missing Country");
        }

        return new Country(givenCountry);
    }

    public override string ToString() => _value;
}