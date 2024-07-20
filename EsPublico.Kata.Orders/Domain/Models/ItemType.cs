using LanguageExt;

namespace EsPublico.Kata.Orders.Domain.Models;

public class ItemType
{
    private readonly string _value;
    private ItemType(string givenType) => _value = givenType;

    public static Either<Error, ItemType> Create(string givenType)
    {
        if (string.IsNullOrWhiteSpace(givenType))
        {
            return new MissingParameter("Missing ItemType");
        }

        return new ItemType(givenType);
    }

    public override string ToString() => _value;
}