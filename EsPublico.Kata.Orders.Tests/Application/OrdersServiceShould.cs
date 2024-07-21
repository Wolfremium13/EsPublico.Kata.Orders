using EsPublico.Kata.Orders.Application;
using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Infrastructure.Apis;
using EsPublico.Kata.Orders.Infrastructure.Repositories;
using EsPublico.Kata.Orders.Tests.Domain.Builders;
using FluentAssertions;
using LanguageExt;
using NSubstitute;

namespace EsPublico.Kata.Orders.Tests.Application;

public class OrdersServiceShould
{
    private readonly FilesRepository _filesRepository;
    private readonly OrdersApi _ordersApi;
    private readonly OrdersRepository _ordersRepository;
    private readonly OrdersService _service;

    public OrdersServiceShould()
    {
        _ordersApi = Substitute.For<OrdersApi>();
        _ordersRepository = Substitute.For<OrdersRepository>();
        _filesRepository = Substitute.For<FilesRepository>();
        _service = new OrdersService(_ordersRepository, _filesRepository, _ordersApi);
    }

    [Fact]
    public async Task IngestFirstOrders()
    {
        var aOrder = new OrderBuilder().WithUuid("1858f59d-8884-41d7-b4fc-88cfbbf00c53").Build();
        var orders = new OrdersBuilder().WithOrder(aOrder).Build();
        _ordersApi.Get().Returns(orders);
        _ordersRepository.Save(orders.Value).Returns(Unit.Default);
        _filesRepository.Save(orders.Value).Returns(Unit.Default);

        await _service.Ingest();

        await _ordersApi.Received(1).Get();
        await _ordersRepository.Received(1).Save(orders.Value);
        await _filesRepository.Received(1).Save(orders.Value);
    }

    [Fact]
    public async Task StopIfErrorIngestingFirstOrders()
    {
        var error = new Error("Some error during ingestion");
        _ordersApi.Get().Returns(error);

        var exception = await Record.ExceptionAsync(() => _service.Ingest());

        exception.Should().BeOfType<Exception>().Which.Message.Should()
            .Be($"Error ingesting first orders: {error.Message}");
    }

    [Fact]
    public async Task IngestNextOrders()
    {
        var aOrder = new OrderBuilder().WithUuid("1858f59d-8884-41d7-b4fc-88cfbbf00c53").Build();
        var nextOrdersLink = NextOrdersLink.Create("http://example.com/orders?page=2").Match(
            link => link,
            _ => throw new Exception("Invalid Next Orders Link")
        );
        var firstOrders = new OrdersBuilder().WithOrder(aOrder).WithNextOrdersLink(nextOrdersLink).Build();
        var ordersWithNextPage = new OrdersBuilder().WithOrder(aOrder).Build();
        _ordersApi.Get().Returns(firstOrders);
        _ordersApi.Get(nextOrdersLink).Returns(ordersWithNextPage);
        _ordersRepository.Save(firstOrders.Value).Returns(Unit.Default);
        _ordersRepository.Save(ordersWithNextPage.Value).Returns(Unit.Default);
        _filesRepository.Save(firstOrders.Value).Returns(Unit.Default);
        _filesRepository.Save(ordersWithNextPage.Value).Returns(Unit.Default);

        await _service.Ingest();

        await _ordersApi.Received(1).Get();
        await _ordersApi.Received(1).Get(nextOrdersLink);
        await _ordersRepository.Received(1).Save(firstOrders.Value);
        await _ordersRepository.Received(1).Save(ordersWithNextPage.Value);
        await _filesRepository.Received(1).Save(firstOrders.Value);
        await _filesRepository.Received(1).Save(ordersWithNextPage.Value);
    }

    [Fact]
    public async Task StopIfErrorIngestingNextOrders()
    {
        var aOrder = new OrderBuilder().WithUuid("1858f59d-8884-41d7-b4fc-88cfbbf00c53").Build();
        var nextOrdersLink = NextOrdersLink.Create("http://example.com/orders?page=2").Match(
            link => link,
            _ => throw new Exception("Invalid Next Orders Link")
        );
        var firstOrders = new OrdersBuilder().WithOrder(aOrder).WithNextOrdersLink(nextOrdersLink).Build();
        _ordersApi.Get().Returns(firstOrders);
        var nextOrdersPageError = new Error("Some error during ingestion");
        _ordersApi.Get(nextOrdersLink).Returns(nextOrdersPageError);
        _ordersRepository.Save(firstOrders.Value).Returns(Unit.Default);
        _filesRepository.Save(firstOrders.Value).Returns(Unit.Default);

        var exception = await Record.ExceptionAsync(() => _service.Ingest());

        exception.Should().BeOfType<Exception>().Which.Message.Should()
            .Be($"Error ingesting orders in page {nextOrdersLink}: {nextOrdersPageError.Message}");
    }
}