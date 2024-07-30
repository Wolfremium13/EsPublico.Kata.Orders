using LanguageExt;

namespace EsPublico.Kata.Orders.Domain;

public class NextOrdersLink
{
    private NextOrdersLink(string givenNextOrdersLink) => Value = givenNextOrdersLink;
    public string Value { get; }

    public static Either<Error, NextOrdersLink> Create(string? givenNextOrdersLink)
    {
        if (string.IsNullOrWhiteSpace(givenNextOrdersLink))
        {
            return new MissingParameter("Missing Next Orders Link");
        }

        if (!Uri.IsWellFormedUriString(givenNextOrdersLink, UriKind.Absolute))
        {
            return new InvalidParameter("Invalid Next Orders Link");
        }

        return new NextOrdersLink(givenNextOrdersLink);
    }
}