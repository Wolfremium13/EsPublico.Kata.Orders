using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Domain.OrderItems;
using LanguageExt;

namespace EsPublico.Kata.Orders.Infrastructure.Repositories;

public interface OrdersRepository
{
    Task<Either<Error, Unit>> Save(List<Order> order);
}