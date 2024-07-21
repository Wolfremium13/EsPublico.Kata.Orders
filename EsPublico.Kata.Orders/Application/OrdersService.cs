using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Infrastructure.Apis;
using EsPublico.Kata.Orders.Infrastructure.Generators;
using EsPublico.Kata.Orders.Infrastructure.Repositories;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace EsPublico.Kata.Orders.Application;

public class OrdersService(
    OrdersRepository ordersRepository,
    FilesRepository filesRepository,
    OrdersApi ordersApi,
    DateTimeGenerator dateTimeGenerator,
    ILogger<OrdersService> logger)
{
    public async Task Ingest()
    {
        var executionDate = dateTimeGenerator.Now();
        var ordersExcutionCounter = 0L;
        logger.LogInformation($"Ingesting orders for {executionDate}");
        var maybeFirstOrders = await FirstOrdersCall(executionDate);
        if (maybeFirstOrders.IsLeft)
        {
            var errorMessage = maybeFirstOrders.LeftAsEnumerable().First().Message;
            throw new Exception($"Error ingesting first orders: {errorMessage}");
        }

        var orders = maybeFirstOrders.RightAsEnumerable().First();
        ordersExcutionCounter += orders.Value.Count;
        logger.LogInformation($"Ingested {ordersExcutionCounter} orders");
        while (orders is OrdersWithNextPage ordersWithNextPage)
        {
            var nextOrders = await NextOrdersCall(executionDate, ordersWithNextPage.NextOrdersLink);
            if (nextOrders.IsLeft)
            {
                throw new Exception(
                    $"Error ingesting orders in page {
                        ordersWithNextPage.NextOrdersLink}: {nextOrders.LeftAsEnumerable().First().Message}");
            }

            orders = nextOrders.RightAsEnumerable().First();
            ordersExcutionCounter += orders.Value.Count;
            logger.LogInformation($"Ingested {ordersExcutionCounter} orders");
        }

        logger.LogInformation("Ingestion finished");
    }

    private async Task<Either<Error, Domain.Orders>> FirstOrdersCall(DateTime executionDate)
    {
        return await (
            from orders in ordersApi.Get().ToAsync()
            from _ in ordersRepository.Save(orders.Value).ToAsync()
            from __ in filesRepository.Save(orders.Value, executionDate).ToAsync()
            select orders
        ).ToEither();
    }

    private async Task<Either<Error, Domain.Orders>> NextOrdersCall(DateTime executionDate,
        NextOrdersLink nextOrdersLink)
    {
        return await (
            from orders in ordersApi.Get(nextOrdersLink).ToAsync()
            from _ in ordersRepository.Save(orders.Value).ToAsync()
            from __ in filesRepository.Save(orders.Value, executionDate).ToAsync()
            select orders
        ).ToEither();
    }
}