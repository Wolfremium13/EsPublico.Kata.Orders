using LanguageExt;

namespace EsPublico.Kata.Orders.Domain.Models;

public record Uuid
{
    private readonly string _value;

    private Uuid(string givenUuid)
    {
        _value = givenUuid;
    }

    public static Either<Error, Uuid> Create(string givenUuid)
    {
        if (!Guid.TryParse(givenUuid, out _))
        {
            return new InvalidParameter("Invalid UUID");
        }

        return new Uuid(givenUuid);
    }

    public override string ToString()
    {
        return _value;
    }
};