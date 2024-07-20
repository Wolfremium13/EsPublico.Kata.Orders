using LanguageExt;

namespace EsPublico.Kata.Orders.Domain.Order;

public class Id
{
    private Id(long givenId) => Value = givenId;

    public long Value { get; }

    public static Either<Error, Id> Create(long givenId)
    {
        if (givenId < 1)
        {
            return new InvalidParameter("Invalid UUID");
        }

        return new Id(givenId);
    }
}