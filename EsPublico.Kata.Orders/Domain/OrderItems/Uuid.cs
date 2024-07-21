using LanguageExt;

namespace EsPublico.Kata.Orders.Domain.OrderItems;

public record Uuid
{
    private readonly string? _value;
    private Uuid(string? givenUuid) => _value = givenUuid;

    public static Either<Error, Uuid> Create(string? givenUuid)
    {
        if (string.IsNullOrWhiteSpace(givenUuid))
        {
            return new MissingParameter("Missing UUID");
        }

        if (!Guid.TryParse(givenUuid, out _))
        {
            return new InvalidParameter("Invalid UUID");
        }

        return new Uuid(givenUuid);
    }

    public override string? ToString() => _value;
};