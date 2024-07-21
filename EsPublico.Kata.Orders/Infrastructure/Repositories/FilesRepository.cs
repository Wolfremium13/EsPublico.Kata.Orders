using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Domain.OrderItems;
using LanguageExt;

namespace EsPublico.Kata.Orders.Infrastructure.Repositories;

public interface FilesRepository
{
    Task<Either<Error, Unit>> Save(List<Order> order, DateTime executionDate);
    Task<Either<Error, Unit>> Save(List<SummaryRecord> summaryRecords);
}