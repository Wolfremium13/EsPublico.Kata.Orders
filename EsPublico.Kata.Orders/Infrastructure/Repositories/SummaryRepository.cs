using EsPublico.Kata.Orders.Domain;
using LanguageExt;

namespace EsPublico.Kata.Orders.Infrastructure.Repositories;

public interface SummaryRepository
{
    Task<Either<Error, List<SummaryRecord>>> GetSummary();
}