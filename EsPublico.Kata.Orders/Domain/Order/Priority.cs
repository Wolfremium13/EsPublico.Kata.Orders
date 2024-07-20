using LanguageExt;

namespace EsPublico.Kata.Orders.Domain.Order;

public class Priority
{
    private readonly string _value;
    private Priority(string givenPriority) => _value = givenPriority;

    public static Either<Error, Priority> Create(string givenPriority)
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

    public override string ToString() => _value;

    private static bool IsPriorityAllowed(string givenPriority) =>
        AllowedPriorities().Any(allowedPriority => allowedPriority == givenPriority);

    private static List<string> AllowedPriorities() => ["C", "H", "M", "L"];
}