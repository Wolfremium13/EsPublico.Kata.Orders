using EsPublico.Kata.Orders.Domain;
using LanguageExt;

namespace EsPublico.Kata.Orders.Infrastructure.Apis;

public interface OrdersApi
{
    Task<Either<Error, Domain.Orders>> Get(NextOrdersLink nextOrdersLink);
    Task<Either<Error, Domain.Orders>> Get();
}