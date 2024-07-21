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
    public async Task GetFirstOrders()
    {
        GivenHttpClientReturns(HttpStatusCode.OK, GivenValidFirstOrdersResponse());

        var response = await _ordersHttpApi.Get();

        response.Match(
            orders =>
            {
                orders.Value.Count.Should().Be(1);
                orders.Value.First().Uuid.ToString().Should().Be("1858f59d-8884-41d7-b4fc-88cfbbf00c53");
            },
            error => error.Should().BeNull()
        );
    }

    [Fact]
    public async Task GetNextOrders()
    {
        var nextOrdersLink = NextOrdersLink.Create("http://example.com/orders?page=2").Match(
            link => link,
            _ => throw new Exception("Next link is null")
        );
        GivenHttpClientReturns(HttpStatusCode.OK, GivenValidNextOrdersResponse());

        var response = await _ordersHttpApi.Get(nextOrdersLink);

        response.Match(
            orders =>
            {
                orders.Value.Count.Should().Be(1);
                orders.Value.First().Uuid.ToString().Should().Be("1858f59d-8884-41d7-b4fc-88cfbbf00c53");
                orders.Should().BeOfType<OrdersWithNextPage>()
                    .Which.NextOrdersLink.ToString().Should().Be(nextOrdersLink.ToString());
            },
            error => error.Should().BeNull()
        );
    }


    [Fact]
    public async Task HandleUnexpectedErrors()
    {
        GivenHttpClientThrowsException();

        var unexpectedError = await _ordersHttpApi.Get();

        unexpectedError.Match(
            orders => orders.Should().BeNull(),
            error => error.Message.Should().Be("Error getting orders: Exception of type 'System.Exception' was thrown.")
        );
    }

    [Fact]
    public async Task HandleWhenSuccessResponseIsNull()
    {
        GivenHttpClientReturns(HttpStatusCode.OK, null!);

        var responseIsEmpty = await _ordersHttpApi.Get();

        responseIsEmpty.Match(
            orders => orders.Should().BeNull(),
            error => error.Message.Should().Be("Success response is null")
        );
    }

    [Fact]
    public async Task HandleWhenResponseIsNotSuccessful()
    {
        GivenHttpClientReturns(HttpStatusCode.BadRequest, null!);

        var badRequest = await _ordersHttpApi.Get();

        badRequest.Match(
            orders => orders.Should().BeNull(),
            error => error.Message.Should().Be("Error getting orders: HTTP status code BadRequest - Bad Request")
        );
    }

    private static SuccessResponse GivenValidFirstOrdersResponse()
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

    private static SuccessResponse GivenValidNextOrdersResponse()
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

        var linksResponse = new LinksResponse
        {
            Next = "http://example.com/orders?page=2"
        };

        return new SuccessResponse
        {
            Content = orderResponses,
            LinksResponse = linksResponse
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