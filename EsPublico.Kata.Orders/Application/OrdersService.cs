using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Infrastructure.Apis;
using EsPublico.Kata.Orders.Infrastructure.Generators;
using EsPublico.Kata.Orders.Infrastructure.Repositories;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace EsPublico.Kata.Orders.Application
{
    public class OrdersService(
        OrdersRepository ordersRepository,
        FilesRepository filesRepository,
        OrdersApi ordersApi,
        DateTimeGenerator dateTimeGenerator,
        ILogger<OrdersService> logger)
    {
        private int _totalOrdersIngested;

        public async Task Ingest()
        {
            var executionDate = InitializeExecutionDate();
            var orders = GetOrThrow(await ProcessOrders(executionDate, ordersApi.Get));
            LogIngestedOrders(orders);
            while (orders is OrdersWithNextPage ordersWithNextPage)
            {
                var maybeNextOrders =
                    await ProcessOrders(executionDate, () => ordersApi.Get(ordersWithNextPage.NextOrdersLink));
                orders = GetOrThrow(maybeNextOrders, ordersWithNextPage);
                LogIngestedOrders(orders);
            }
            LogFinish();
        }

        private void LogFinish()
        {
            var finishTime = dateTimeGenerator.Now();
            logger.LogInformation($"Ingestion finished at {finishTime} with {_totalOrdersIngested} orders");
        }

        private static Domain.Orders GetOrThrow(Either<Error, Domain.Orders> maybeOrders)
        {
            return maybeOrders.Match(
                Left: error => throw new Exception($"Error ingesting first orders from config: {error.Message}"),
                Right: orders => orders
            );
        }

        private static Domain.Orders GetOrThrow(Either<Error, Domain.Orders> maybeOrders,
            OrdersWithNextPage ordersWithNextPage)
        {
            return maybeOrders.Match(
                Left: error =>
                    throw new Exception(
                        $"Error ingesting orders in page {ordersWithNextPage.NextOrdersLink}: {error.Message}"),
                Right: orders => orders
            );
        }

        private void LogIngestedOrders(Domain.Orders orders)
        {
            _totalOrdersIngested += orders.Value.Count;
            logger.LogInformation($"Ingested {_totalOrdersIngested} orders");
        }

        private DateTime InitializeExecutionDate()
        {
            var executionDate = dateTimeGenerator.Now();
            logger.LogInformation($"Ingesting orders for {executionDate}");
            return executionDate;
        }

        private async Task<Either<Error, Domain.Orders>> ProcessOrders(DateTime executionDate,
            Func<Task<Either<Error, Domain.Orders>>> getOrdersFunc)
        {
            var maybeOrders = await (
                from orders in getOrdersFunc().ToAsync()
                from _ in ordersRepository.Save(orders.Value).ToAsync()
                from __ in filesRepository.Save(orders.Value, executionDate).ToAsync()
                select orders
            );
            return maybeOrders;
        }
    }
}