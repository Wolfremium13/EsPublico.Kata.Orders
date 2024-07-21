using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Infrastructure.Apis;
using EsPublico.Kata.Orders.Infrastructure.Repositories;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace EsPublico.Kata.Orders.Application;

public class OrdersService(
    OrdersRepository ordersRepository,
    FilesRepository filesRepository,
    OrdersApi ordersApi,
    ILogger<OrdersService> logger)
{
    public async Task Ingest()
    {
        var maybeFirstOrders = await FirstOrdersCall();
        if (maybeFirstOrders.IsLeft)
        {
            var errorMessage = maybeFirstOrders.LeftAsEnumerable().First().Message;
            throw new Exception($"Error ingesting first orders: {errorMessage}");
        }

        var orders = maybeFirstOrders.RightAsEnumerable().First();
        logger.LogInformation($"Ingested {orders.Value.Count} orders");
        while (orders is OrdersWithNextPage ordersWithNextPage)
        {
            var nextOrders = await NextOrdersCall(ordersWithNextPage.NextOrdersLink);
            if (nextOrders.IsLeft)
            {
                throw new Exception(
                    $"Error ingesting orders in page {
                        ordersWithNextPage.NextOrdersLink}: {nextOrders.LeftAsEnumerable().First().Message}");
            }

            orders = nextOrders.RightAsEnumerable().First();
            logger.LogInformation($"Ingested {orders.Value.Count} orders");
        }
        logger.LogInformation("Ingestion finished");
    }

    private async Task<Either<Error, Domain.Orders>> FirstOrdersCall()
    {
        return await (
            from orders in ordersApi.Get().ToAsync()
            from _ in ordersRepository.Save(orders.Value).ToAsync()
            from __ in filesRepository.Save(orders.Value).ToAsync()
            select orders
        ).ToEither();
    }

    private async Task<Either<Error, Domain.Orders>> NextOrdersCall(NextOrdersLink nextOrdersLink)
    {
        return await (
            from orders in ordersApi.Get(nextOrdersLink).ToAsync()
            from _ in ordersRepository.Save(orders.Value).ToAsync()
            from __ in filesRepository.Save(orders.Value).ToAsync()
            select orders
        ).ToEither();
    }
}