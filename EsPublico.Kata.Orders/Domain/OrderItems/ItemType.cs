using LanguageExt;

namespace EsPublico.Kata.Orders.Domain.OrderItems;

public class ItemType
{
    private ItemType(string givenType) => Value = givenType;
    public string Value { get; }
    
    public static Either<Error, ItemType> Create(string? givenType)
    {
        if (string.IsNullOrWhiteSpace(givenType))
        {
            return new MissingParameter("Missing ItemType");
        }

        return new ItemType(givenType);
    }
}