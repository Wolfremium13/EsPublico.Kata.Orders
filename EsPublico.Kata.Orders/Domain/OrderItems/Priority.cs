using LanguageExt;

namespace EsPublico.Kata.Orders.Domain.OrderItems;

public class Priority
{
    private Priority(string givenPriority) => Value = givenPriority;
    public string Value { get; }

    public static Either<Error, Priority> Create(string? givenPriority)
    {
        if (string.IsNullOrWhiteSpace(givenPriority))
        {
            return new MissingParameter("Missing Priority");
        }

        if (IsPriorityAllowed(givenPriority))
        {
            return new Priority(givenPriority);
        }

        return new InvalidParameter(
            $"Invalid Priority: {givenPriority}, expected one of {string.Join(", ", AllowedPriorities())}");
    }

    private static bool IsPriorityAllowed(string? givenPriority) =>
        AllowedPriorities().Any(allowedPriority => allowedPriority == givenPriority);

    private static List<string> AllowedPriorities() => ["C", "H", "M", "L"];
}