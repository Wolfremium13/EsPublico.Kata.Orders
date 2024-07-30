using LanguageExt;

namespace EsPublico.Kata.Orders.Domain.OrderItems;

public record Uuid
{
    private Uuid(string givenUuid) => Value = givenUuid;
    public string Value { get; }

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

    public Guid ToGuid() => Guid.Parse(Value);
};