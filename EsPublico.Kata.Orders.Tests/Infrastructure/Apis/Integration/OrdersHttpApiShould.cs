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
        const int port = 3000;
        _server = WireMockServer.Start(port);
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
        GivenAOkResponseWithNextPage();

        var result = await _ordersHttpApi.Get();

        result.Match(
            orders =>
            {
                const int numberOfOrdersPerPage = 100;
                orders.Value.Count.Should().Be(numberOfOrdersPerPage);
                orders.Should().BeOfType<OrdersWithNextPage>()
                    .Which.NextOrdersLink.ToString().Should().Be($"{_server.Url}/v1/orders?page=2&max-per-page=100");
            },
            error => error.Should().BeNull()
        );
    }

    [Fact]
    public async Task GetOrdersFromLink()
    {
        GivenAnOkResponseWithoutNextPage();
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
                orders.Should().BeOfType<Orders.Domain.Orders>();
            },
            error => error.Should().BeNull()
        );
    }

    private void GivenAnOkResponseWithoutNextPage()
    {
        var requestNextOrdersMatcher = Request.Create()
            .WithHeader("Accept", "application/json")
            .WithUrl($"{_server.Url}/v1/orders?page=2")
            .UsingGet();
        var nextOrdersResponsePath =
            Path.Combine("Infrastructure", "Apis", "Integration", "Resources", "ok_next_response.json");
        var okResponse = File.ReadAllText(nextOrdersResponsePath);
        var responseNextOrdersWithoutNextPage = Response.Create().WithBody(okResponse);
        _server
            .Given(requestNextOrdersMatcher)
            .RespondWith(responseNextOrdersWithoutNextPage);
    }

    private void GivenAOkResponseWithNextPage()
    {
        var okResponsePath =
            Path.Combine("Infrastructure", "Apis", "Integration", "Resources", "ok_first_response.json");
        var okResponse = File.ReadAllText(okResponsePath);
        var requestFirstOrdersMatcher = Request.Create()
            .WithHeader("Accept", "application/json")
            .WithUrl($"{_server.Url}/v1/orders")
            .UsingGet();
        var responseFirstOrdersWithNextPage = Response.Create().WithBody(okResponse);
        _server
            .Given(requestFirstOrdersMatcher)
            .RespondWith(responseFirstOrdersWithNextPage);
    }
}