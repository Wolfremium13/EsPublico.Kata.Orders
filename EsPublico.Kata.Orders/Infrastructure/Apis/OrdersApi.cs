using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Domain.OrderItems;
using LanguageExt;

namespace EsPublico.Kata.Orders.Infrastructure.Apis;

public interface OrdersApi
{
    Task<Either<Error, List<Order>>> Get(PageNumber pageNumber);
}