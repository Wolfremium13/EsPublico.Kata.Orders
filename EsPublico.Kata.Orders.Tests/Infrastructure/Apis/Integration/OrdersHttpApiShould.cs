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
    // TODO: hay que hacer el next link
    private readonly OrdersHttpApi _ordersHttpApi;
    private readonly WireMockServer _server;

    public OrdersHttpApiShould()
    {
        _server = WireMockServer.Start();
        var apiSettings = new ApiSettings
        {
            BaseUrl = $"{_server.Url}/v1/orders"
        };
        var httpClientFactory = new HttpClientFactory();
        _ordersHttpApi = new OrdersHttpApi(httpClientFactory, apiSettings);
    }

    public void Dispose() => _server.Stop();

    [Fact]
    public async Task GetOrdersFromConfig()
    {
        GivenAnOkResponse();

        var result = await _ordersHttpApi.Get();

        result.Match(
            orders =>
            {
                const int numberOfOrdersPerPage = 100;
                orders.Value.Count.Should().Be(numberOfOrdersPerPage);
            },
            error => error.Should().BeNull()
        );
    }

    [Fact]
    public async Task GetOrdersFromLink()
    {
        GivenAnOkResponse();
        var nextOrdersLink = NextOrdersLink.Create($"{_server.Url}/v1/orders?page=2")
            .Match(
                link => link,
                error => throw new Exception($"Error creating next orders link: {error.Message}")
            );
        var result = await _ordersHttpApi.Get(nextOrdersLink);

        result.Match(
            orders =>
            {
                const int numberOfOrdersPerPage = 100;
                orders.Value.Count.Should().Be(numberOfOrdersPerPage);
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
            .WithPath("/v1/orders")
            .UsingGet();
        var responseBuilder = Response.Create().WithBody(okResponse);
        _server
            .Given(requestMatcher)
            .RespondWith(responseBuilder);
    }
}