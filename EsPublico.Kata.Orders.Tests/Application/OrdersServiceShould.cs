using EsPublico.Kata.Orders.Application;
using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Domain.OrderItems;
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
    public async Task IngestOrders()
    {
        var pageNumber = new PageNumberBuilder().WithValue(1).Build();
        var orders = new List<Order> { new OrderBuilder().WithUuid("1858f59d-8884-41d7-b4fc-88cfbbf00c53").Build() };
        _ordersApi.Get(pageNumber).Returns(orders);
        _ordersRepository.Save(orders).Returns(Unit.Default);
        _filesRepository.Save(orders).Returns(Unit.Default);

        await _service.Ingest(pageNumber);

        await _ordersApi.Received(1).Get(pageNumber);
        await _ordersRepository.Received(1).Save(orders);
        await _filesRepository.Received(1).Save(orders);
    }

    [Fact]
    public async Task StopIfErrorIngestingOrders()
    {
        var pageNumber = new PageNumberBuilder().WithValue(2).Build();
        var error = new Error("Some error during ingestion");
        _ordersApi.Get(pageNumber).Returns(error);

        var exception = await Record.ExceptionAsync(() => _service.Ingest(pageNumber));

        exception.Should().BeOfType<Exception>().Which.Message.Should()
            .Be($"Error ingesting orders in page {pageNumber}: {error.Message}");
    }
}