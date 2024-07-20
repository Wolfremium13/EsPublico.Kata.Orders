using System.Net;
using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Infrastructure.Apis;
using EsPublico.Kata.Orders.Infrastructure.Apis.Models;
using EsPublico.Kata.Orders.Infrastructure.Config;
using EsPublico.Kata.Orders.Tests.Infrastructure.Fixtures;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using IHttpClientFactory = EsPublico.Kata.Orders.Infrastructure.Apis.IHttpClientFactory;

namespace EsPublico.Kata.Orders.Tests.Infrastructure.Apis.Unit;

public class OrdersHttpApiShould
{
    private const string BaseUrl = "http://localhost:5000";

    private readonly ApiSettings _apiSettings = new()
    {
        BaseUrl = BaseUrl
    };

    private readonly IHttpClientFactory _httpClientFactory;

    private readonly OrdersHttpApi _ordersHttpApi;

    public OrdersHttpApiShould()
    {
        _httpClientFactory = Substitute.For<IHttpClientFactory>();
        _ordersHttpApi = new OrdersHttpApi(_httpClientFactory, _apiSettings);
    }

    [Fact]
    public async Task GetOrders()
    {
        var pageNumber = PageNumber.Create(1).Match(
            value => value,
            _ => throw new Exception("Invalid PageNumber")
        );
        GivenHttpClientReturns(HttpStatusCode.OK, GivenValidResponse());

        var response = await _ordersHttpApi.Get(pageNumber);

        response.Match(
            orders =>
            {
                orders.Count.Should().Be(1);
                orders.First().Uuid.ToString().Should().Be("1858f59d-8884-41d7-b4fc-88cfbbf00c53");
            },
            error => error.Should().BeNull()
        );
    }

    [Fact]
    public async Task HandleUnexpectedErrors()
    {
        var pageNumber = PageNumber.Create(1).Match(
            value => value,
            _ => throw new Exception("Invalid PageNumber")
        );
        GivenHttpClientThrowsException();

        var unexpectedError = await _ordersHttpApi.Get(pageNumber);

        unexpectedError.Match(
            orders => orders.Should().BeNull(),
            error => error.Message.Should().Be("Error getting orders: Exception of type 'System.Exception' was thrown.")
        );
    }

    [Fact]
    public async Task HandleWhenSuccessResponseIsNull()
    {
        var pageNumber = PageNumber.Create(1).Match(
            value => value,
            _ => throw new Exception("Invalid PageNumber")
        );
        GivenHttpClientReturns(HttpStatusCode.OK, null!);

        var responseIsEmpty = await _ordersHttpApi.Get(pageNumber);

        responseIsEmpty.Match(
            orders => orders.Should().BeNull(),
            error => error.Message.Should().Be("Success response is null")
        );
    }

    [Fact]
    public async Task HandleWhenResponseIsNotSuccessful()
    {
        var pageNumber = PageNumber.Create(1).Match(
            value => value,
            _ => throw new Exception("Invalid PageNumber")
        );
        GivenHttpClientReturns(HttpStatusCode.BadRequest, null!);

        var badRequest = await _ordersHttpApi.Get(pageNumber);

        badRequest.Match(
            orders => orders.Should().BeNull(),
            error => error.Message.Should().Be("Error getting orders: HTTP status code BadRequest - Bad Request")
        );
    }

    private static SuccessResponse GivenValidResponse()
    {
        List<OrderResponse> orderResponses =
        [
            new OrderResponse
            {
                Uuid = "1858f59d-8884-41d7-b4fc-88cfbbf00c53",
                Id = 1,
                Region = "Region",
                Country = "Country",
                ItemType = "ItemType",
                SalesChannel = "SalesChannel",
                Priority = "H",
                Date = "7/27/2012",
                ShipDate = "7/27/2012",
                UnitsSold = 1,
                UnitPrice = 1,
                UnitCost = 1,
                TotalRevenue = 1,
                TotalCost = 1,
                TotalProfit = 1
            }
        ];
        return new SuccessResponse
        {
            Content = orderResponses
        };
    }

    private void GivenHttpClientReturns(HttpStatusCode statusCode, object content)
    {
        var httpResponseMessage = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(JsonConvert.SerializeObject(content))
        };
        var httpClient = new HttpClient(new MockHttpMessageHandler(httpResponseMessage));
        _httpClientFactory.CreateClient().Returns(httpClient);
    }

    private void GivenHttpClientThrowsException()
    {
        _httpClientFactory.CreateClient().Throws<Exception>();
    }
}