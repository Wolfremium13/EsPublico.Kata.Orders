using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Domain.OrderItems;
using EsPublico.Kata.Orders.Infrastructure.Apis.Models;
using EsPublico.Kata.Orders.Infrastructure.Config;
using LanguageExt;
using Newtonsoft.Json;

namespace EsPublico.Kata.Orders.Infrastructure.Apis
{
    public class OrdersHttpApi(IHttpClientFactory httpClientFactory, ApiSettings apiSettings) : OrdersApi
    {
        public async Task<Either<Error, List<Order>>> Get(PageNumber pageNumber)
        {
            try
            {
                var response = await SendRequest(pageNumber);
                if (!response.IsSuccessStatusCode)
                {
                    return new Error($"Error getting orders: HTTP status code {response.StatusCode}" +
                                     $" - {response.ReasonPhrase}");
                }
                var successResponse = await MapToSuccessResponse(response);
                return successResponse != null
                    ? MapResponseToOrders(successResponse.Content)
                    : new Error("Success response is null");
            }
            catch (Exception e)
            {
                return new Error($"Error getting orders: {e.Message}");
            }
        }

        private async Task<HttpResponseMessage> SendRequest(PageNumber pageNumber)
        {
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await client.GetAsync($"{apiSettings.BaseUrl}/orders?page={pageNumber}");
            return response;
        }

        private static async Task<SuccessResponse?> MapToSuccessResponse(HttpResponseMessage response)
        {
            var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);
            using var jsonReader = new JsonTextReader(reader);
            var serializer = new JsonSerializer();
            var successResponse = serializer.Deserialize<SuccessResponse>(jsonReader);
            return successResponse;
        }

        private static Either<Error, List<Order>> MapResponseToOrders(List<OrderResponse> orderResponses)
        {
            var maybeOrders = orderResponses.Select(orderResponse => Order.Create(
                uuid: orderResponse.Uuid,
                id: orderResponse.Id,
                region: orderResponse.Region,
                country: orderResponse.Country,
                itemType: orderResponse.ItemType,
                salesChannel: orderResponse.SalesChannel,
                priority: orderResponse.Priority,
                date: orderResponse.Date,
                shipDate: orderResponse.ShipDate,
                unitsSold: orderResponse.UnitsSold,
                unitPrice: orderResponse.UnitPrice,
                unitCost: orderResponse.UnitCost,
                totalRevenue: orderResponse.TotalRevenue,
                totalCost: orderResponse.TotalCost,
                totalProfit: orderResponse.TotalProfit
            )).ToList();

            var errors = maybeOrders
                .Where(maybeOrder => maybeOrder.IsLeft)
                .SelectMany(maybeOrder => maybeOrder.LeftToSeq())
                .ToList();

            if (errors.Any())
            {
                return new Error(string.Join(", ", errors.Select(e => e.Message)));
            }

            var orders = maybeOrders
                .Where(maybeOrder => maybeOrder.IsRight)
                .Select(maybeOrder => maybeOrder.RightToSeq().First())
                .ToList();

            return orders;
        }
    }
}