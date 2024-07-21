using EsPublico.Kata.Orders.Infrastructure.Apis;
using EsPublico.Kata.Orders.Infrastructure.Repositories;
using LanguageExt;

namespace EsPublico.Kata.Orders.Application;

public class OrdersService(
    OrdersRepository ordersRepository,
    FilesRepository filesRepository,
    OrdersApi ordersApi)
{
    public async Task Ingest()
    {
        var result = await (
            from orders in ordersApi.Get().ToAsync()
            from _ in ordersRepository.Save(orders).ToAsync()
            from __ in filesRepository.Save(orders).ToAsync()
            select Unit.Default
        ).ToEither();

        result.Match(
            Right: _ =>
            {
                /* Success - do nothing */
            },
            Left: error => throw new Exception($"Error ingesting orders: {error.Message}")
        );
    }
}