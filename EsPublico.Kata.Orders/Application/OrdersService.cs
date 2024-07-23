using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Infrastructure.Apis;
using EsPublico.Kata.Orders.Infrastructure.Generators;
using EsPublico.Kata.Orders.Infrastructure.Repositories;
using LanguageExt;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace EsPublico.Kata.Orders.Application
{
    public class OrdersService(
        OrdersRepository ordersRepository,
        FilesRepository filesRepository,
        OrdersApi ordersApi,
        DateTimeGenerator dateTimeGenerator,
        ILogger<OrdersService> logger)
    {
        public async Task Ingest()
        {
            var executionDate = InitializeExecutionDate();
            var maybeOrders = await ProcessOrders(executionDate, ordersApi.Get);
            if (maybeOrders.IsLeft)
            {
                var errorMessage = maybeOrders.LeftAsEnumerable().First().Message;
                throw new Exception($"Error ingesting first orders: {errorMessage}");
            }
            var orders = maybeOrders.RightAsEnumerable().First();
            var ordersExecutionCounter = orders.Value.Count;
            logger.LogInformation($"Ingested {ordersExecutionCounter} orders");
            while (orders is OrdersWithNextPage ordersWithNextPage)
            {
                maybeOrders = await ProcessOrders(executionDate, () => ordersApi.Get(ordersWithNextPage.NextOrdersLink));
                if (maybeOrders.IsLeft)
                {
                    var errorMessage = maybeOrders.LeftAsEnumerable().First().Message;
                    throw new Exception($"Error ingesting orders in page {ordersWithNextPage.NextOrdersLink}: {errorMessage}");
                }

                orders = maybeOrders.RightAsEnumerable().First();
                ordersExecutionCounter += orders.Value.Count;
                logger.LogInformation($"Ingested {ordersExecutionCounter} orders");
            }
            logger.LogInformation("Ingestion finished");
        }

        private DateTime InitializeExecutionDate()
        {
            var executionDate = dateTimeGenerator.Now();
            logger.LogInformation($"Ingesting orders for {executionDate}");
            return executionDate;
        }

        private async Task<Either<Error, Domain.Orders>> ProcessOrders(DateTime executionDate, Func<Task<Either<Error, Domain.Orders>>> getOrdersFunc)
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
