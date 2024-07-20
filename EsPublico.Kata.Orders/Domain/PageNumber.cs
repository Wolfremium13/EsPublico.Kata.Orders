using LanguageExt;

namespace EsPublico.Kata.Orders.Domain;

public class PageNumber
{
    private PageNumber(int givenPageNumber) => Value = givenPageNumber;
    public int Value { get; }


    public static Either<Error, PageNumber> Create(int givenPageNumber)
    {
        if (givenPageNumber < 1)
        {
            return new InvalidParameter("Invalid Page Number");
        }

        return new PageNumber(givenPageNumber);
    }

    public override string ToString() => Value.ToString();
}