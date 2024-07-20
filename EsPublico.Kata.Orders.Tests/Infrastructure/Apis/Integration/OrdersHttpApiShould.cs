using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Infrastructure.Apis;
using EsPublico.Kata.Orders.Infrastructure.Config;
using FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace EsPublico.Kata.Orders.Tests.Infrastructure.Apis.Integration;

public class OrdersHttpApiShould : IDisposable
{
    private readonly OrdersHttpApi _ordersHttpApi;
    private readonly WireMockServer _server;

    public OrdersHttpApiShould()
    {
        _server = WireMockServer.Start();
        var apiSettings = new ApiSettings
        {
            BaseUrl = _server.Url!
        };
        var httpClientFactory = new HttpClientFactory();
        _ordersHttpApi = new OrdersHttpApi(httpClientFactory, apiSettings);
    }

    public void Dispose() => _server.Stop();

    [Fact]
    public async Task GetOrders()
    {
        var pageNumber = PageNumber.Create(1).Match(
            value => value,
            _ => throw new Exception("Invalid PageNumber")
        );
        GivenAnOkResponse();

        var result = await _ordersHttpApi.Get(pageNumber);

        result.Match(
            orders =>
            {
                const int numberOfOrdersPerPage = 100;
                orders.Count.Should().Be(numberOfOrdersPerPage);
            },
            error => error.Should().BeNull()
        );
    }

    private void GivenAnOkResponse()
    {
        var okResponsePath = Path.Combine("Infrastructure", "Apis", "Integration", "Resources", "ok_response.json");
        var okResponse = File.ReadAllText(okResponsePath);
        var requestMatcher = Request.Create()
            .WithHeader("Accept", "application/json")
            .WithPath("/orders")
            .WithParam("page", "1")
            .UsingGet();
        var responseBuilder = Response.Create().WithBody(okResponse);
        _server
            .Given(requestMatcher)
            .RespondWith(responseBuilder);
    }
}